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

	// Play 2 seconds of each tone
	//short := sr.N(time.Second/10)
  long := sr.N(time.Second/2)

  ch := make(chan struct{})
  for index, s := range val {
    fmt.Println(s)
    fmt.Println(index)
    sounds := []beep.Streamer{
      beep.Take(long, sine),
      beep.Callback(func() {
        ch <- struct{}{}
      }),
    }
	  speaker.Play(beep.Seq(sounds...))
	  <-ch
  }
}
