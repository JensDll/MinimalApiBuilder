import { Compression } from './compression'

export default defineNuxtConfig({
  devtools: { enabled: true },
  css: ['~/assets/main.css'],
  postcss: {
    plugins: {
      tailwindcss: {},
    },
  },
  vite: {
    plugins: [Compression()],
  },
  nitro: {
    output: {
      publicDir: '../wwwroot',
    },
  },
  experimental: {
    inlineSSRStyles: false,
  },
})
