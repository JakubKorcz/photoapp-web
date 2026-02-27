# ETAP 1: Budowanie (Kompilacja)
# Używamy oficjalnego obrazu Go na bazie Alpine Linux (bardzo lekki)
FROM golang:1.21-alpine AS builder

# Instalujemy zależności systemowe niezbędne do skompilowania bimg i libvips
# build-base to kompilator C (gcc), a vips-dev to nagłówki biblioteki vips
RUN apk add --no-cache vips-dev build-base

# Ustawiamy folder roboczy wewnątrz kontenera
WORKDIR /app

# Kopiujemy pliki definicji modułów i pobieramy biblioteki Go
COPY go.mod go.sum ./
RUN go mod download

# Kopiujemy resztę kodu źródłowego
COPY . .

# Kompilujemy aplikację. 
# CGO_ENABLED=1 jest konieczne, bo bimg łączy się z biblioteką napisaną w C
RUN CGO_ENABLED=1 GOOS=linux go build -o main .


# ETAP 2: Uruchamianie (Finalny obraz)
FROM alpine:latest

# W obrazie końcowym potrzebujemy tylko samej biblioteki vips (bez narzędzi deweloperskich)
RUN apk add --no-cache vips

WORKDIR /root/

# Kopiujemy tylko skompilowaną binarkę z pierwszego etapu
COPY --from=builder /app/main .

# Tworzymy folder na zdjęcia, żeby program miał gdzie zapisywać
RUN mkdir output

# Informujemy, na jakim porcie działa aplikacja
EXPOSE 8080

# Uruchamiamy program
CMD ["./main"]