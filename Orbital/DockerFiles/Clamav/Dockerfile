FROM debian:stable-slim

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update && apt-get install -y clamav clamav-daemon
RUN freshclam --quiet

ENV BACKEND_ID=clamav