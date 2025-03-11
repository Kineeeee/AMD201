<script setup>
import UrlShortenerForm from '../components/UrlShortenerForm.vue';
import UrlList from '../components/UrlList.vue';
</script>

<template>
  <div>
    <h1>Rút Gọn URL</h1>
    <UrlShortenerForm />
    <UrlList />
  </div>
</template>

<style scoped>
h1 {
  text-align: center;
  margin-top: 20px;
}
</style>
