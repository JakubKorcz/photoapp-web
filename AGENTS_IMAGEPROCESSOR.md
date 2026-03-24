# AGENTS_IMAGEPROCESSOR.md - PhotoApp.ImageProcessor

## Overview

PhotoApp.ImageProcessor is a **Go microservice** that processes images. Responsible for:
- **Image resizing** - Reduces images to max 1600px dimension
- **Format conversion** - Converts to WebP (modern web format)
- **Metadata stripping** - Removes EXIF, ICC for smaller files

**Language**: Go 1.21+  
**Library**: bimg v1.1.9 (libvips wrapper)  
**Port**: 8080 (internal), 8082 (exposed)

---

## Project Structure

```
PhotoApp.ImageProcessor/
├── main.go       # All application code
├── go.mod        # Module: PhotoApp.ImageProcessor
├── go.sum        # Dependencies
├── Dockerfile    # Multi-stage Docker build
└── output/       # Created at runtime (processed images)
```

---

## The `/import` Endpoint

### Request
- **Method**: POST only (405 for other methods)
- **Body**: Raw image binary (any format: JPEG, PNG, TIFF, GIF, BMP)

### Processing Logic

```
1. Read image binary from request body
2. Get image dimensions (width, height)
3. Determine orientation:
   - Portrait (height > width): resize height to 1600px
   - Landscape (width >= height): resize width to 1600px
4. Convert to WebP with 82% quality, strip metadata
5. Save to ./output/photo_<nanoseconds>.webp
6. Return status message with filename and duration
```

### Image Options

| Parameter | Value | Description |
|-----------|-------|-------------|
| Width | 1600 or 0 (auto) | Max width |
| Height | 1600 or 0 (auto) | Max height |
| Crop | false | Preserve aspect ratio |
| Type | WEBP | Output format |
| Quality | 82 | Compression level |
| StripMetadata | true | Remove EXIF/ICC |

### Response
```text
Koniec przetwarzania! Plik zapisany jako: photo_1709123456789012345.webp (Czas: 234ms)
```

---

## Build & Run

### Prerequisites
- Go 1.21+
- libvips (for bimg)

### Build
```bash
cd PhotoApp/PhotoApp.ImageProcessor

# Download dependencies
go mod tidy

# Build binary
go build -o imageprocessor main.go
```

### Run
```bash
# Run directly
go run main.go

# Or run binary
./imageprocessor

# Image processor available at http://localhost:8080
```

### Docker Build
```bash
cd PhotoApp/PhotoApp.ImageProcessor

# Build image
docker build -t photoapp-imageprocessor .

# Run container
docker run -p 8082:8080 photoapp-imageprocessor
```

---

## Docker Configuration

```yaml
photo-app-imageprocessor:
  build: ./PhotoApp.ImageProcessor
  ports:
    - "8082:8080"
  networks:
    - app-network
  environment:
    - Api__BaseUrl=http://photo-app-api:8080
```

### Dockerfile (Multi-stage)
```dockerfile
# Stage 1: Builder
FROM golang:1.26-alpine AS builder
RUN apk add vips-dev build-base gcc musl-dev libc-dev
WORKDIR /app
COPY go.mod go.sum ./
RUN go mod download
COPY *.go ./
RUN CGO_ENABLED=1 GOOS=linux go build -o main .

# Stage 2: Runtime
FROM alpine:latest
RUN apk add vips
WORKDIR /app
COPY --from=builder /app/main .
RUN mkdir output
EXPOSE 8080
CMD ["./main"]
```

---

## Communication with API

```
┌─────────────────┐         ┌────────────────────────────┐
│   PhotoApp.Api  │  HTTP   │  photo-app-imageprocessor  │
│                 │ ──────► │  POST /import              │
│  Port: 8080     │         │  - Receives image bytes    │
│                 │ ◄────── │  - Returns filename + time │
└─────────────────┘         └────────────────────────────┘
```

Both services on same Docker network.

---

## Important Notes

- Uses **CGO** because bimg wraps libvips (C library)
- Images saved locally in `./output/` directory
- Not yet integrated with main API (standalone)
- Alpine-based image for small footprint
- Images NOT uploaded to MinIO yet (future feature)
