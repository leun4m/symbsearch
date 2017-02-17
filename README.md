# SymbSearch ![logo][logo]

## No longer Electron-App

The most critical point was the huge storage and the huge amount of RAM which SymbSearch needed as Electron-app. I think, this objection was right. It is just a char table. So I rewrote the application in **Visual C#** – of course, it is no longer possible to build it for the other platforms like Linux and Mac, but who needs an app which is just so big to be platform independent.
If you want to see the Electron-app just look at [v0.4.2]. I will indeed no longer work on that.
I hope, you agree on that decision.
With SymbSearch you can easily get the special unicode letter or symbol you need.

[v0.4.2]: https://github.com/leun4m/symbsearch/tree/v0.4.2

## How it works

You just need to start the application and can **show** and **hide** the window about the **shortcut**.

Default it is `Ctrl` + `Alt` + `W`

You just need to **type in** the name of the letter e.g. "delta" and you get - voilá a "Δ and δ". You just need to select the one you want (via `↑` | `↓`) and press `Enter` and so you have it in your clipboard. Pretty cool, isn't it?!

[logo]: https://github.com/Leun4m/symbsearch/raw/master/style/icon32.png
