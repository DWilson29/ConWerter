package main

import (
	"fmt"
	"os"
	"strconv"
	"time"

	"github.com/gopxl/beep/v2"
	"github.com/gopxl/beep/v2/generators"
	"github.com/gopxl/beep/v2/speaker"
)

func main() {
  f := 
  1000
	sr := beep.SampleRate(48000)
	speaker.Init(sr, 4800)

	sine, err := generators.SineTone(sr, f)
	if err != nil {
		panic(err)
	}

	// Play 2 seconds of each tone
	beep := sr.N(time.Second/10)
  long := sr.N(time.Second/2)

	ch := make(chan struct{})
	sounds := []beep.Streamer{
		beep.Callback(print("sine")),
		beep.Take(two, sine),
		beep.Callback(func() {
			ch <- struct{}{}
		}),
	}
	speaker.Play(beep.Seq(sounds...))
	<-ch
}

// a simple clousure to wrap fmt.Println
func print(s string) func() {
	return func() {
		fmt.Println(s)
	}
}
