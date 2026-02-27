package main

import (
	"fmt"
	"io"
	"net/http"
	"os"
	"path/filepath"
	"time"
	"github.com/h2non/bimg"
)

func main() {
	outputDir := "./output"
	os.MkdirAll(outputDir, os.ModePerm)

	http.HandleFunc("/import", func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, "Only POST method is available", http.StatusMethodNotAllowed)
			return
		}

		fmt.Println("--- Start przetwarzania ---")
		start := time.Now()

		buffer, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "Błąd odczytu obrazu", http.StatusBadRequest)
			return
		}

		image := bimg.NewImage(buffer)

		size, err := image.Size()
		if err != nil {
			http.Error(w, "Błąd odczytywania metadanych obrazka", http.StatusInternalServerError)
			return 
		}

		var options bimg.Options

		if size.Height > size.Width {
			options = bimg.Options{
				Width:         0,
				Height:        1600,
				Crop:          false,
				Type:          bimg.WEBP,
				Quality:       82,
				StripMetadata: true,
			}
		} else {
			options = bimg.Options{
				Width:         1600,
				Height:        0,
				Crop:          false,
				Type:          bimg.WEBP,
				Quality:       82,
				StripMetadata: true,
			}
		}

		newImage, err := image.Process(options)
		if err != nil {
			http.Error(w, "Błąd przetwarzania", http.StatusInternalServerError)
			return
		}

		// w.Header().Set("Content-Type", "image/webp")
		// w.Write(newImage)

		fileName := fmt.Sprintf("photo_%d.webp", time.Now().UnixNano())
		filePath := filepath.Join(outputDir, fileName)

		duration := time.Since(start)
		
		err = os.WriteFile(filePath, newImage, 0644)
		if err != nil {
			http.Error(w, "Błąd zapisu pliku", http.StatusInternalServerError)
			return
		}

		statusMsg := fmt.Sprintf("Koniec przetwarzania! Plik zapisany jako: %s (Czas: %v)\n", fileName, duration)
		
		fmt.Print(statusMsg)
		w.Write([]byte(statusMsg))
	})

	fmt.Println("Go Image Converter startuje na porcie :8080...")
	http.ListenAndServe(":8080", nil)
}