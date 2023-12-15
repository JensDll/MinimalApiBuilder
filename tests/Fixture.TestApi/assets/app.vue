<template>
  <div class="prose mx-auto w-full max-w-6xl justify-center px-4">
    <header>
      <nav class="space-x-3">
        <NuxtLink to="/">Home</NuxtLink>
        <NuxtLink to="/first-page">First page</NuxtLink>
        <NuxtLink to="/second-page">Second page</NuxtLink>
      </nav>
    </header>
    <main>
      <NuxtPage />
    </main>
    <footer></footer>
  </div>
</template>
