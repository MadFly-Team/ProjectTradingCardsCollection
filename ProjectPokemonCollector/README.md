# Pokémon Card Collector

A separate Windows Forms Pokémon card collector based on the existing MTG Collector UI. It supports card-name lookup, set browsing, card images, printing/finish details, collection folders, price display, and the webcam/OCR workflow.

Search results and selected cards in Cards in Set include PriceCharting graded values. Set `PRICECHARTING_API_TOKEN` to your paid API token in Windows user environment variables, restart the app/Visual Studio, and use **Load / change graded values...** to confirm the exact printing. See the [shared setup and licensing guidance](../README.md#graded-card-values).

The General column covers grades 6, 7/7.5, 8/8.5, and 9 across graders; it is not PSA-specific. Grade 10 has separate PSA, CGC, and TAG values when supplied. Unsupported/missing values remain N/A. Prices show USD and estimated GBP with a source link and lookup time. Confirmed products are cached only for the session, separately for each physical variant; collection values are unchanged.

In Cards in Set, the variants table appears below both the image and the complete card details.

The app opens on Browse Sets and loads all sets once at startup. Switching back to Browse Sets does not reload them. Use Reload Sets to refresh manually or retry a failed load.

## Data and storage

- Card and set data is retrieved from the public [TCGdex API](https://tcgdex.dev/), which does not require an API key.
- Set release dates are loaded from each set's detail endpoint because the set list omits them. The first set-list load therefore takes longer; set metadata and full card details are cached for the current session.
- Concurrent requests for the same card or set share one download. Requests are limited to six at a time, with up to three attempts for temporary network/server errors and rate limiting. Failed lookups are not cached, so selecting a card again retries it.
- Prices are shown when supplied by TCGdex's TCGplayer or Cardmarket data. They can be missing for some cards and finishes; GBP values are estimates using the app's exchange-rate feed.
- Cards in Set loads full card details and prices before displaying the list or thumbnails, with download progress shown. Both views show normal pricing when available, otherwise a labeled holo price; N/A means neither price is supplied. The initial load takes longer, and subsequent loads reuse the session cache.
- Collections are stored separately at `%APPDATA%\PokemonCollector\collections.json`; the MTG app's data is not shared.
- OCR requires the included `eng.traineddata` file, which is copied to the build output.
- Image downloads retry temporary network errors, timeouts, rate limits, and server failures up to three attempts. Only successful images are cached, so selecting a card again retries a failed download. Missing or failed images show a No Image placeholder; download failures are logged to diagnostic trace output. The HTTP timeout remains at its default of 100 seconds per attempt.

## Physical variants

- The variants tables show one selectable row per TCGdex physical variant: finish, subtype (such as unlimited or shadowless), stamps, and size. This card's variants appear immediately; Load All Variants retrieves all exact-name printings without the previous 100-record limit.
- Select a row to view its own USD/EUR pricing. The Add to Collection dialog uses a variant selector and preserves the row selection. Holo, reverse holo, editions, and stamped versions are saved and removed independently.
- Detailed variants use their own pricing when provided. Missing prices remain unavailable rather than borrowing a different edition's price. Plain, unambiguous finishes may use matching card-level finish prices. Set-list and thumbnail prices remain card-level estimates; use a physical variant row for its specific price.
- When detailed variants are unavailable, only the provider's reported finish choices are offered. A First Edition availability flag alone does not establish every edition/finish combination; these choices are explicitly labeled edition unspecified. Variant coverage and artwork depend on TCGdex.
- Existing saved entries keep their quantities, prices, notes, and holo flags. They are labeled Legacy holo/non-holo (exact variant unknown) and are never automatically merged with a newly identified physical variant. Opening a saved exact variant restores it when still listed by the provider.

## Build

Open `ProjectPokemonCollector.csproj` in Visual Studio or build it with the .NET 10 SDK on Windows:

```powershell
dotnet build .\ProjectPokemonCollector.csproj
```
