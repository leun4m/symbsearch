#Installation guide for building the application
***

#Linux (Tested with Ubuntu 16.04)
##Mono and GTK#
1. Install mono: [http://www.mono-project.com/download/]
2. Install GTK#: `sudo apt-get install gtk-sharp2`
3. Test if everything is fine, e.g. compile an example: `mcs hello.cs -pkg:gtk-sharp-2.0`

##MonoDevelop
###Latest version (recommended)
1. Install Flatpak: [https://flatpak.org/getting.html]
2. Install MonoDevelop:
 1. `flatpak install --user --from https://download.mono-project.com/repo/monodevelop.flatpakref`
 2. `Configure this as new remote 'gnome' [y/n]: y`
 3. `Found in remote gnome, do you want to install it? [y/n]: y`

###Stable version (might also work)
1. `sudo apt install monodevelop`
