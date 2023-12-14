<template>
  <div class="prose mx-auto w-full max-w-6xl justify-center px-4">
    <header>
      <span class="text-xl font-medium">Static Site Sample</span>
      <nav class="mt-2 space-x-2">
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
