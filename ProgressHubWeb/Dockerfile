FROM --platform=$BUILDPLATFORM node:18.18.1-alpine as builder

RUN mkdir /ProgressHubWeb
WORKDIR /ProgressHubWeb

RUN npm install -g @angular/cli@16.2.6

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
CMD ["ng", "serve", "--host", "0.0.0.0"]