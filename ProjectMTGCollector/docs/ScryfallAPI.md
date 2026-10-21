# Scryfall API Usage — Technical Breakdown

This document describes exactly how the application integrates with Scryfall, including endpoints, data models, pagination, error handling, caching, and UI flows.

## Overview
The app calls public Scryfall REST endpoints to fetch:
- Single card by name
- All printings/variants for a card
- Sets
- Cards within a set (paginated)
- Image assets for thumbnails and detail views

Network calls are encapsulated in `ScryfallService` and mapped to models under `ProjectMTGCollector/Models`.

## Endpoints

### 1. Cards by name (best match)
- Endpoint: `GET https://api.scryfall.com/cards/named`
- Query: `?fuzzy={name}` or `?exact={name}`
- App flow: In `Search Card` tab, `Search` triggers `ScryfallService.GetCardByNameAsync(name)` using fuzzy matching. Returns a single `Card`.

### 2. Search and list cards (used for set cards)
- Endpoint: `GET https://api.scryfall.com/cards/search`
- Query examples:
  - Cards in set: `?q=e:{setCode}` (exact set code)
- Pagination: Scryfall returns a `List` response with `has_more` and `next_page`. The app iterates pages until the result is complete, reporting `PageProgress` to update the marquee/progress bar.
- App flow: In `Cards in Set`, `ScryfallService.GetCardsBySetAsync(setCode, progress)` constructs the set query, iterates pages, aggregates `Card` list.

### 3. Sets
- Endpoint: `GET https://api.scryfall.com/sets`
- Pagination: Full list is returned (may be paginated), the app aggregates.
- App flow: In `Browse Sets`, `ScryfallService.GetAllSetsAsync()` loads all sets and sorts by `ReleasedAt`.

### 4. Printings / All variants of a card
- Endpoint pattern: Use search with `oracleid` or name-based search
  - Typical: `GET /cards/search?q=oracleid:{oracleId}` (preferred when available)
  - Fallback: `GET /cards/search?q=!”{card name}”` combined with other filters (name-based may include non-exact matches)
- App flow: In both `Search Card` and `Cards in Set` details, `ScryfallService.GetAllPrintingsAsync(cardName)` fetches all printings mapped to `Card` list. The UI composes variant description from `finishes`, `frame_effects`, `promo`, `border_color`, and shows prices.

### 5. Images
- Returned in `image_uris` for single-faced cards or in each face of `card_faces` for multi-faced.
- The app prefers:
  - Thumbnail: `image_uris.small`
  - Detail: `image_uris.normal`
  - Alternatives: `large`, `png`
- `ImageCacheService` downloads and caches by URL for reuse.

## Models
Key models in `ProjectMTGCollector/Models` map to Scryfall fields:
- `Card`: maps properties like `name`, `set`, `set_name`, `collector_number`, `type_line`, `oracle_text`, `image_uris`, `card_faces`, `prices`, `colors`, `rarity`, `released_at`, `legalities`, `promo`, `finishes`, `frame_effects`, `border_color`, `artist`.
- `Set`: maps `code`, `name`, `released_at`, `set_type`, `card_count`, `digital`.
- `ScryfallResponse`: encapsulates list responses: `data`, `has_more`, `next_page`, plus total-like tracking for progress.

UI computations:
- Variant description: combine `promo`, `frame_effects`, `finishes`, `border_color`.
- Price text: prefer `prices.usd`, fallback to `prices.eur`; foil variants use `prices.usd_foil` or `prices.eur_foil`.
- Multi-face: use first face’s `image_uris.normal` for display.

## Service Layer (`ScryfallService`)
Responsibilities:
- Build query URLs
- Perform HTTP GET
- Deserialize JSON into models
- Iterate pagination for `/cards/search` and `/sets`
- Emit `PageProgress` during long-running fetches

Typical patterns:
- Name lookup: build `/cards/named?fuzzy={name}` and deserialize into `Card`.
- Sets list: call `/sets`, merge/paginate all pages to `List<Set>`.
- Cards in set: call `/cards/search?q=e:{code}`, loop while `has_more` is true, using `next_page` URLs.
- Printings: search by oracle id if available; otherwise fallback by name.

## Pagination
Scryfall search returns:
- `has_more`: boolean
- `next_page`: absolute URL for next page
- `data`: array of `Card` or `Set`

The app uses a loop:
1. Request initial search URL
2. Add `data` to aggregate list
3. If `has_more` true, request `next_page`
4. Report `PageProgress` with loaded count and current page
5. Stop when `has_more` is false

Progress UI:
- `prgLoadingSetCards` uses marquee until `total` becomes known; then uses continuous style and updates value.

## Rate Limiting & Courtesy
Scryfall has generous limits but requests should be paced:
- The app uses sequential pagination rather than bursting many parallel requests.
- Avoid repeated queries in short intervals; `ImageCacheService` prevents re-fetching identical images.

## Error Handling
- Network exceptions bubble to UI with `MessageBox.Show` for user-friendly messages.
- Name search not found: handled by showing "Card not found." and clearing the display.
- Variants/load errors: show a warning and keep UI usable.

## Image Caching (`ImageCacheService`)
- Caches images by URL in-memory and/or disk (depending on implementation).
- On image request, returns cached copy if present, otherwise downloads and stores.
- Reduces bandwidth and speeds up subsequent displays.

## UI Integration Summary
- `Search Card` tab
  - `GetCardByNameAsync` ? fill details panel
  - `GetAllPrintingsAsync` ? fill `listViewVariants`
- `Browse Sets` tab
  - `GetAllSetsAsync` ? fill `listViewSets` and set details
- `Cards in Set` tab
  - `GetCardsBySetAsync` ? fill `flowLayoutPanelCards` thumbnails or `listViewSetCards`
  - Sorting on `collector_number`, `prices`, `rarity`, `name`
  - Variant load for selected card: `GetAllPrintingsAsync`

## Data Transformations
- Collector number sorting: extract numeric prefix from `collector_number` (e.g., `new string(c.TakeWhile(char.IsDigit).ToArray())`).
- Price normalization: prefer USD; convert EUR approximately when sorting by price.
- Rarity ordering: map `common`?1, `uncommon`?2, `rare`?3, `mythic`?4, others=0.

## Legalities
- `legalities` is a map of format?status; app displays only entries where value is `legal`.

## Known Edge Cases
- Multi-faced cards require reading from `card_faces[0].image_uris.normal` when `image_uris` is null.
- Some cards lack `prices`; UI shows `N/A`.
- Some printings have finishes like `etched` or border colors not `black` — included in variant description.

## Testing & Validation
- Basic correctness validated via UI:
  - Search various card names (including misspellings) to confirm fuzzy matching
  - Load sets and inspect cards count matches Scryfall
  - Verify pagination by checking progress and final counts
  - Confirm variants lists match Scryfall website
- Build pipeline confirms project compiles with .NET 10

## References
- Scryfall API docs: https://scryfall.com/docs/api
- Cards named: https://scryfall.com/docs/api/cards/named
- Cards search: https://scryfall.com/docs/api/cards/search
- Sets: https://scryfall.com/docs/api/sets

