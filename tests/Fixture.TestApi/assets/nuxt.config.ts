export default defineNuxtConfig({
  devtools: { enabled: true },
  css: ['~/assets/main.css'],
  postcss: {
    plugins: {
      tailwindcss: {},
    },
  },
  routeRules: {
    '/': { redirect: '/first-page' },
  },
  experimental: {
    inlineSSRStyles: false,
  },
})
