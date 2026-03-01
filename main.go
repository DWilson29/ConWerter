package main

import (
	"fmt"
	"os"
  "time"
  "strings"
	"github.com/gopxl/beep/v2"
	"github.com/gopxl/beep/v2/generators"
	"github.com/gopxl/beep/v2/speaker"
)


func main() {
  val := os.Args[1]
  lower := strings.ToLower(val)

  f := 500.0
	sr := beep.SampleRate(48000)
	speaker.Init(sr, 4800)

  sine, err := generators.SineTone(sr, f)
	if err != nil {
		panic(err)
	} 
	empty, err := generators.SineTone(sr, 0.0)
	if err != nil {
		panic(err)
	} 
	// Play 2 seconds of each tone
	short := sr.N(time.Second/9)
  long := sr.N(time.Second/3)

  var morseCode = map[rune][]int{
    'a': {short, long},
    'b': {long, short, short, short},
    'c': {long, short, long, short},
    'd': {long, short, short},
    'e': {short},
    'f': {short, short, long, short},
    'g': {long, long, short},
    'h': {short, short, short, short},
    'i': {short, short},
    'j': {short, long, long ,long},
    'k': {long, short, long},
    'l': {short, long, short, short},
    'm': {long, long},
    'n': {long, short},
    'o': {long, long, long},
    'p': {short, long, long},
    'q': {long, long, short, long},
    'r': {short, long, short},
    's': {short, short, short},
    't': {long},
    'u': {short, short, long},
    'v': {short, short, short, long},
    'w': {short, long, long},
    'x': {long, short, short, long},
    'y': {long, short, long, long},
    'z': {long, long, short, short},
    '1': {short, long, long, long, long},
    '2': {short, short, long, long, long},
    '3': {short, short, short, long, long},
    '4': {short, short, short, short, long},
    '5': {short, short, short, short, short},
    '6': {long, short, short, short, short},
    '7': {long, long, short, short, short},
    '8': {long, long, long, short, short},
    '9': {long, long, long, long, short},
    '0': {long, long, long, long, long},
  }

  ch := make(chan struct{})
  for _, s := range lower {
    morse := morseCode[s]
    var sounds []beep.Streamer

    for _, delay := range morse {
      fmt.Println(delay)
      sounds = append(sounds, beep.Take(delay, sine))
      sounds = append(sounds, beep.Take(short, empty))
    }

    sounds = append(sounds, beep.Callback(func(){
      ch <- struct{}{}
    }))
	  speaker.Play(beep.Seq(sounds...))
	  <-ch
  }
}
