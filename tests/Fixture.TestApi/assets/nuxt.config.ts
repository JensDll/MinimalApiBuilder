export default defineNuxtConfig({
  devtools: { enabled: true },
  css: ['~/assets/main.css'],
  postcss: {
    plugins: {
      tailwindcss: {},
    },
  },
  nitro: {
    compressPublicAssets: true,
    output: {
      publicDir: '../wwwroot',
    },
  },
  experimental: {
    inlineSSRStyles: false,
  },
})
