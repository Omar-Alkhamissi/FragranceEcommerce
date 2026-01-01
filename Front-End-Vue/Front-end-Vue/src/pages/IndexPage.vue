
<template>
  <q-page class="flex flex-center column q-pa-md">
    <div class="text-h3 text-primary q-mb-lg">Home</div>
    <img
      alt="Spade Logo"
      src="img/Spade.png"
      style="width: 200px; height: 200px; object-fit: contain;"
      @error="onImageError"
    >
  </q-page>
</template>

<script setup>
const onImageError = (event) => {
  console.error('Image failed to load:', event.target.src);

}
</script>
