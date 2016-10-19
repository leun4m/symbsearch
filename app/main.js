const {app, BrowserWindow, globalShortcut, ipcMain} = require('electron')
const notifier = require('node-notifier')
//The config file
const config = require('./config.json')

let win

function createWindow () {
  // Create the browser window.
  win = new BrowserWindow({width: config.width, height: config.height, icon: __dirname + '/style/icon32.png', frame: false, resizable: false})
  // and load the index.html of the app.
  win.loadURL(`file://${__dirname}/index.html`)
  win.hide()
  //win.webContents.openDevTools()
  // Emitted when the window is closed.
  win.on('closed', () => {
    win = null
  })
}

app.on('ready', () => {
  createWindow()
  let shcut = config.shortcut;
   // Register a 'CommandOrControl+X' shortcut listener.
   const ret = globalShortcut.register(shcut, () => {
     console.log(shcut + ' is pressed')
     if (win.isVisible()) {
       win.hide()
       console.log("Window hidden")
     } else {
       win.show()
       console.log("Window shown")
     }
   })

   if (!ret) {
     console.log('registration failed')
   }

   // Check whether a shortcut is registered.
   //console.log(globalShortcut.isRegistered('Super+S'))
   console.log('SymbSearch is ready!')

   notifier.notify({
     title: 'SymbSearch is ready!',
     message: 'You can run it by ' + shcut,
     icon: __dirname + '/style/icon32.png'
   });
})

// Quit when all windows are closed.
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit()
  }
})

app.on('activate', () => {
  if (win === null) {
    createWindow()
  }
})

app.on('will-quit', () => {
  // Unregister all shortcuts.
  globalShortcut.unregisterAll()
})

ipcMain.on('asynchronous-message', (event, msg) => {
  switch (msg) {
    case 'hide':
      win.blur()
      win.hide()
      break
    case 'quit':
      win.close()
      break
    default:
      console.log("GET msg: " + msg)
  }
})
