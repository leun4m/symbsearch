# Installation guide for building the application
***

## Linux 
Tested with Ubuntu 16.04
### Mono and GTK#
1. Install mono: [http://www.mono-project.com/download/]
2. Install GTK#: `sudo apt-get install gtk-sharp2`
3. Test if everything is fine, e.g. build an example: [http://www.mono-project.com/docs/getting-started/mono-basics/#gtk-hello-world] 

### MonoDevelop
#### Latest version (recommended)
1. Install Flatpak: [https://flatpak.org/getting.html]
2. Install MonoDevelop:
  * `flatpak install --user --from https://download.mono-project.com/repo/monodevelop.flatpakref`
  * `Configure this as new remote 'gnome' [y/n]: y`
  * `Found in remote gnome, do you want to install it? [y/n]: y`

#### Stable version (might also work)
1. `sudo apt install monodevelop`

***

## Windows
Coming soon ...
