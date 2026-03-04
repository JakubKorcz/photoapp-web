FROM golang:1.26-alpine AS builder

RUN apk add --no-cache vips-dev build-base

WORKDIR /app

COPY go.mod go.sum ./
RUN go mod download

COPY . .

RUN CGO_ENABLED=1 GOOS=linux go build -o main .

FROM alpine:latest

RUN apk add --no-cache vips

WORKDIR /root/

COPY --from=builder /app/main .

RUN mkdir output

EXPOSE 8080

CMD ["./main"]