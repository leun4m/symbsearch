# Change Log

## [Unreleased]
### Added
- Ctrl-Backspace function to remove whole words
- Highlight searchbox text on "recall"
- Version number to database (via `//v` available)

### Changed
- Disabled beep
- Fixed issues on selection if list is empty

## [0.5.2] - 2017-02-26
### Added
- Enhancement experience while using keyboard

### Changed
- Code structure
- Filter & search

## [0.5.0] - 2017-02-17
- **Translated tool in Visual C#**

## [0.4.0] - 2017-02-12
### Added
- Database: symbols.json
  - Latin-B
  - Special
  - Box Drawing
  - Block Elements
  - Mathematics (Intergrated Multiplication and Division Sign there from Latin-1)
- ignore parameters to `dist` script in `package.json`

### Changed
- Database: symbols.json
  - Latin+ => Latin-A
  - greek => Greek
  - arrows => Arrows
  - ASCII => Latin Basic
- Optimized design
  - The searchbox no longer moves to the top...
  - some minor changes
- Using `select` instead of `radio` for the categories

## [0.3.6] - 2017-01-28
### Added
- Database: symbols.json
  - ASCII symbols
  - Latin-1 symbols
  - Math symbols
  - Letters+ (Latin-1 & Latin Extended-A)

## Changed
- Database: symbols.json from *Object Array* -> *Array*
  (to minimize storage and better vision)

## [0.3.2] - 2016-10-19
### Added
- `config.json` to change the shortcut

### Changed
- README instructions
- Global shortcut set to `Ctrl` / `Cmd` + `Alt` + `W`
  (`Ctrl` + `Q` is often used for closing apps ...)
- Optimized start-session
  (Window is going to be build before calling it)
- Uses now *node-notifier* instead of *Tray*

## [0.3.0] - 2016-10-06
### Added
- Database: symbols.json
  - Arrows
  - Greek letters
- Global shortcut (set to `Ctrl` + `Q`) to show and hide window
- via `alt` you can choose the category (you have to press it 2x)
- via `↑` and `↓` you can select the desired symbol
- `Enter` copies the selected symbol to clipboard
- Long names are cutted off
- Readme
- Changelog
- Licence (MIT)

[Unreleased]: https://github.com/Leun4m/symbsearch/compare/v0.5.2...HEAD
[0.5.2]: https://github.com/Leun4m/symbsearch/tree/v0.5.2
[0.5.0]: https://github.com/Leun4m/symbsearch/tree/v0.5.0
[0.4.0]: https://github.com/Leun4m/symbsearch/tree/v0.4.0
[0.3.6]: https://github.com/Leun4m/symbsearch/tree/v0.3.6
[0.3.2]: https://github.com/Leun4m/symbsearch/tree/v0.3.2
[0.3.0]: https://github.com/Leun4m/symbsearch/tree/v0.3.0
