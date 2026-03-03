package main

import (
  "os"
  "strings"
  "github.com/gotk3/gotk3/gtk"
)


func main() {
  gtk.Init(nil)

  win := setup_window("ConWerter")
  box := setup_box(gtk.ORIENTATION_VERTICAL)

  win.ShowAll()

  gtk.Main()

  //val := os.Args[1]
  //lower := strings.ToLower(val)

  //playSound(lower)
}
