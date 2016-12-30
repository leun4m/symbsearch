# Change Log

## NEW
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

[Unreleased]: https://github.com/Leun4m/symbsearch/compare/v0.3.2...HEAD
[0.3.2]: https://github.com/Leun4m/symbsearch/tree/v0.3.2
[0.3.0]: https://github.com/Leun4m/symbsearch/tree/v0.3.0
