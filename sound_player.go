package main

import (
  "time"
  "fmt"
	"github.com/gopxl/beep/v2"
	"github.com/gopxl/beep/v2/generators"
	"github.com/gopxl/beep/v2/speaker"
)  

var f = 500.0
var sr = beep.SampleRate(48000)

var short = sr.N(time.Second/9)
var long = sr.N(time.Second/3)

var morseCode = map[rune][]rune{
    'a': {'.', '-'},
    'b': {'-', '.', '.', '.'},
    'c': {'-', '.', '-', '.'},
    'd': {'-', '.', '.'},
    'e': {'.'},
    'f': {'.', '.', '-', '.'},
    'g': {'-', '-', '.'},
    'h': {'.', '.', '.', '.'},
    'i': {'.', '.'},
    'j': {'.', '-', '-' ,'-'},
    'k': {'-', '.', '-'},
    'l': {'.', '-', '.', '.'},
    'm': {'-', '-'},
    'n': {'-', '.'},
    'o': {'-', '-', '-'},
    'p': {'.', '-', '-'},
    'q': {'-', '-', '.', '-'},
    'r': {'.', '-', '.'},
    's': {'.', '.', '.'},
    't': {'-'},
    'u': {'.', '.', '-'},
    'v': {'.', '.', '.', '-'},
    'w': {'.', '-', '-'},
    'x': {'-', '.', '.', '-'},
    'y': {'-', '.', '-', '-'},
    'z': {'-', '-', '.', '.'},
    '1': {'.', '-', '-', '-', '-'},
    '2': {'.', '.', '-', '-', '-'},
    '3': {'.', '.', '.', '-', '-'},
    '4': {'.', '.', '.', '.', '-'},
    '5': {'.', '.', '.', '.', '.'},
    '6': {'-', '.', '.', '.', '.'},
    '7': {'-', '-', '.', '.', '.'},
    '8': {'-', '-', '-', '.', '.'},
    '9': {'-', '-', '-', '-', '.'},
    '0': {'-', '-', '-', '-', '-'},
}

func playSound(text string) {
	speaker.Init(sr, 4800)

  sine, err := generators.SineTone(sr, f)
	if err != nil {
		panic(err)
	} 
	empty, err := generators.SineTone(sr, 0.0)
	if err != nil {
		panic(err)
	} 

  ch := make(chan struct{})
  for _, s := range text {
    morse := morseCode[s]
    var sounds []beep.Streamer

    for _, delay := range morse {
      var timeDelay int
      fmt.Printf("%c", delay)

      if delay == '-' {
        timeDelay = long
      } else if delay == '.'{
        timeDelay = short
      } else {
        panic(fmt.Sprintf("Unexpected value in map! %c", delay))
      }

      sounds = append(sounds, beep.Take(timeDelay, sine))
      sounds = append(sounds, beep.Take(short, empty))
    }
    fmt.Printf("\n")
    sounds = append(sounds, beep.Callback(func(){
      ch <- struct{}{}
    }))
	  speaker.Play(beep.Seq(sounds...))
	  <-ch
  }
}
