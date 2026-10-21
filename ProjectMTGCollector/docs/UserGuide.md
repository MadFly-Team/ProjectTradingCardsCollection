# MTG Card Database Viewer — User Guide

## Overview
MTG Card Database Viewer is a Windows desktop application (WinForms, .NET 10) for browsing Magic: The Gathering cards and sets using the Scryfall API. It lets you:
- Search individual cards
- Explore sets and view their cards
- Inspect print variants and pricing
- Manage personal card collections with folders
- Scan cards from a webcam for quick lookup

This guide explains the UI, workflows, and features across all tabs and menus.

## System Requirements
- Windows 10 or newer
- .NET 10 Desktop Runtime
- Internet access (Scryfall API)
- Optional: Webcam for card scanning

## Main Window Layout
The application uses a TabControl with 4 tabs:
- Search Card
- Browse Sets
- Cards in Set
- My Collections

A top MenuStrip provides File and Help menus.

---

## File Menu
- Preferences…
  - Opens theme selection (Light or other available themes).
  - Applies colors to panels, lists, labels, and buttons.
- Exit
  - Closes the application.

## Help Menu
- About
  - Shows application information.

---

## Search Card Tab
Use this tab to find a card by name and view detailed information.

### Controls
- Card Image
  - Double-click to open in `Image Viewer`.
- Card name input
  - Enter name and press Enter or click `Search`.
- Search
  - Queries Scryfall for the best match.
- Load Variants
  - Loads all printings/variants for the current card.
- Add to Collection
  - Opens `Add to Collection` dialog for the current card.
- Scan Webcam
  - Opens `Webcam Scanner` to detect card via a camera.

### Details Panel
Displays:
- Name
- Current Value (from prices)
- Mana Cost and CMC
- Type line
- Oracle text
- Power/Toughness (if applicable)
- Set info, Rarity, Artist
- Prices (USD/EUR/Tix)
- Legalities (formats where legal)
- Variants & Printings list
  - Columns: Set, Type/Finish, Collector #, Nonfoil, Foil, Released
  - Selecting a variant updates the details panel

### Tips
- If a card has multiple faces, images and text reflect the primary face.
- Use `Load Variants` under the image for quick access.

---

## Browse Sets Tab
Search and load sets, view details, and open the set’s cards.

### Controls
- Filter sets…
  - Live text filter across set name and code.
- Load All Sets
  - Fetches sets from Scryfall.
- Sets ListView
  - Columns: Name, Code, Cards, Released, Type
  - Click columns to sort; double-click a set to open its cards.
- Set Details Panel
  - Displays Name, Code, Type, Card Count, Release Date
  - `View Cards in Set` button opens the `Cards in Set` tab.

### Sorting
- Column click sorting is available on the sets list.
- Default load sorts by Released (newest first).

---

## Cards in Set Tab
Browse cards from the selected set with two view modes: Thumbnails (grid) and List (table). Inspect a selected card on the right panel.

### Header Controls
- Current Set label
  - Shows the set name, code, and load/progress info during fetch.
- Sort by
  - Options: Card Number, Value (Price), Rarity, Name.
- Sort order toggle
  - `Ascending` / `Descending`. Helper label explains current sort.
- View mode
  - `Thumbnails` or `List`.
- Thumb Size
  - `Small`, `Medium`, `Large` — affects thumbnail tile sizes.

### Jump to Card Number
- Type a collector number in `Jump to card #` to immediately select and scroll to that card in List or Thumbnails view, without sorting or filtering the set.
- Matches the full collector number; suffixes such as `123a` are supported (case-insensitive). The input keeps focus so you can continue typing.
- An inline message shows the matching card or explains that no match was found. Press Enter to jump again after scrolling or changing views.
- Clearing the box leaves the list unchanged. Opening another set clears the box.

### Thumbnails View
- Left panel: `flowLayoutPanelCards`
  - Each tile shows: card image, `CollectorNumber - Name`, and price.
  - Click any area of a tile to select the card.
- Right panel: Selected Card Info
  - Image (double-click opens `Image Viewer`)
  - Name and Current Value
  - Type and Mana Cost
  - Variant info and finishes
  - Text and Set/Rarity/Collector #/Artist
  - `Load All Variants` button (populates variants list below)
  - `Add to Collection` button
  - Variants list (same columns as Search Card tab)

### List View
- Left panel: `listViewSetCards` (visible when View=List)
  - Columns: `#` (Collector Number), Name, Rarity, Price, Set
  - Selecting a row updates the right panel with the selected card.
- Opening a set defaults to List view and replaces the previous list with the loaded cards before the tab opens; no sort change is needed.

### Loading Indicator
- A small progress bar shows page load progress while fetching set cards.

### Calculated British Pound Values
- Card details, thumbnails, set lists, variants, collection cards, and collection totals include estimated GBP values alongside original prices.
- The app fetches the latest published USD/EUR/GBP reference rates from Frankfurter when the main window opens. These are daily reference rates, not real-time trading quotes; weekends and holidays use the latest published business day. The card value panel shows the rate date.
- GBP is calculated from USD where available, otherwise EUR. Foil and nonfoil values are calculated separately, rounded to two decimal places for display, and marked with `~` as estimates rather than UK market prices.
- Internet access is required. If rate loading fails, the app shows a warning and marks GBP unavailable while retaining original prices. Reopen the app to retry.
- New collection entries save their source currency. Older entries have no currency metadata, so their saved values are labelled currency unknown and GBP is unavailable rather than guessed. A total containing such entries is also marked unavailable. Entries without any price remain N/A and contribute zero to totals, as before.
- Collection totals convert each known source currency before summing, include quantities, and round only the final GBP total. Saved source prices are not replaced by converted values.

### Sorting Behavior
- Card Number: numeric sort using the numeric portion of collector # (e.g., `123a` ? 123)
- Value: uses USD or EUR fallback (EUR roughly converted)
- Rarity: common ? mythic (ascending) or reversed (descending)
- Name: A-Z or Z-A

---

## My Collections Tab
Organize your personal collections, folders, and cards.

### Left Panel: Collections List
- Columns: Collection, Cards
- Selecting a collection shows details and its folders/cards.
- Buttons:
  - New Collection
  - Edit (enabled when a collection is selected)
  - Delete (enabled when a collection is selected)

### Top Details Panel
- Shows collection name, description, and stats (e.g., value).

### Middle Left: Folder Tree
- Displays folders within the selected collection.
- Selecting a folder shows its cards.
- Buttons:
  - New Folder (enabled when a collection is selected)
  - Delete Folder (enabled when a folder is selected)

### Right: Cards List
- Columns: Card Name, Set, Collector #, Qty, Foil, Condition, Value
- Remove Card button

### Auto-Refresh
- App polls collection storage periodically and refreshes UI on change.

---

## Webcam Scanner
Scan a physical card using a webcam to assist with recognition.

### Usage
1. Click `Scan Webcam` on the Search Card tab.
2. Choose your camera (virtual devices supported: Camo, DroidCam, EpocCam, OBS Virtual Camera, etc.).
3. Align the card; the form will attempt to detect the name/art and perform a lookup.
4. Confirm the result and optionally add to collection.

### Tips
- Use good lighting and avoid glare.
- Many virtual webcam apps offer resolution/focus controls.

---

## Image Viewer
Double-click a card image (Search Card or Cards in Set) to open the image viewer.
- Shows the card image in a larger window.
- Useful for inspecting details.

---

## Variants & Pricing
Pricing fields show available USD/EUR (and Tix in details). Some printings may be nonfoil/foil-only or etched variants.
- Finishes reflect available print types
- Frame effects indicate showcase/extended art/borderless, etc.

---

## Themes & Preferences
Open `Preferences…`:
- Choose a color theme.
- Applied to the main form, menus, tabs, panels, labels, buttons, and list views.

---

## Keyboard Shortcuts
- Enter in Card name input: triggers Search.

---

## Known Behaviors
- Some cards (split, double-faced) show face-specific text/images.
- Collector number sort uses the numeric part for ordering.
- Prices from Scryfall may be missing; `N/A` is shown.

---

## Troubleshooting
- Build/Run issues: Ensure .NET 10 Desktop Runtime is installed.
- No results: Verify internet connection; check card name spelling.
- Webcam not listed: Install a virtual webcam driver or use a supported camera.
- Images slow to load: Uses an image cache; first load may take time.

---

## Data Sources & Attribution
- Card and set data provided by Scryfall (https://scryfall.com/docs/api).
- Respect Scryfall’s terms of use and rate limits.

---

## Release Notes
Recent additions:
- Cards in Set: List view mode with columns (`#`, Name, Rarity, Price, Set).
- Cards in Set: Thumbnail sizes (Small/Medium/Large) switchable.
- Tiles show `CollectorNumber - Name` under images.

---

## Feedback
Use the About dialog for app info. For issues or feature requests, contact the developer or report via the repository where applicable.
