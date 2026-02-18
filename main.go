package main

import (
	"fmt"
	"os"
  "time"
	"github.com/gopxl/beep/v2"
	"github.com/gopxl/beep/v2/generators"
	"github.com/gopxl/beep/v2/speaker"
)


func main() {
  val := os.Args[1]
  f := 1000.0
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
	short := sr.N(time.Second/10)
  long := sr.N(time.Second/2)
  longest := sr.N(time.Second)

  var morseCode = map[rune][]int{
    'a': {short, long},
    'b': {long, short, short, short},
    'c': {short, short, short},
    'd': {long, short, short},
    'e': {short},
    'f': {short, long, short},
    'g': {long, long, short},
    'h': {short, short, short, short},
    'i': {short, short},
    'j': {long, short, long, short},
    'k': {long, short, long},
    'l': {longest},
    'm': {long, long},
    'n': {long, short},
    'o': {short, short},
    'p': {short, short, short, short, short},
    'q': {short, short, long, short},
    'r': {short, long, short},
    's': {short, short, short},
    't': {long},
    'u': {short, short, long},
    'v': {short, short, short, long},
    'w': {short, long, long},
    'x': {short, long, short, short},
    'y': {long, short, long, long},
    'z': {long, long, short, short},
  }

  ch := make(chan struct{})
  for _, s := range val {
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
