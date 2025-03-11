<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const urls = ref([]);
const isLoading = ref(false);
const baseUrl = "http://localhost:5043"; // Chỉnh theo backend của bạn

const fetchUrls = async () => {
  isLoading.value = true;
  try {
    const response = await axios.get(`${baseUrl}/list`);
    urls.value = response.data.map(url => ({
      ...url,
      fullShortUrl: `${baseUrl}/${url.shortUrl}`
    }));
  } catch (err) {
    console.error("❌ Lỗi khi tải danh sách URL:", err);
  } finally {
    isLoading.value = false;
  }
};

// Gọi API khi trang được tải
onMounted(fetchUrls);
</script>


<template>
  <div class="url-list">
    <h2>Danh sách URL đã rút gọn</h2>

    <!-- Nút Reload -->
    <button @click="fetchUrls" :disabled="isLoading">
      <span v-if="isLoading">⏳ Đang tải...</span>
      <span v-else>🔄 Tải lại danh sách</span>
    </button>

    <!-- Hiển thị danh sách -->
    <ul v-if="urls.length">
      <li v-for="url in urls" :key="url.shortUrl">
        <p><strong>🔗 URL gốc:</strong> 
          <a :href="url.originalUrl" target="_blank">{{ url.originalUrl }}</a>
        </p>
        <p><strong>🔗 URL rút gọn:</strong> 
          <a :href="url.fullShortUrl" target="_blank">{{ url.fullShortUrl }}</a>
        </p>
      </li>
    </ul>

    <!-- Nếu danh sách trống -->
    <p v-else>📭 Không có URL nào được rút gọn.</p>
  </div>
</template>


<style scoped>
.url-list {
  text-align: center;
  margin: 20px auto;
  max-width: 600px;
  padding: 20px;
  background: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

h2 {
  color: #333;
}

button {
  margin-bottom: 15px;
  padding: 10px 16px;
  cursor: pointer;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  transition: 0.3s;
}

button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}

button:hover:not(:disabled) {
  background-color: #0056b3;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  margin-bottom: 12px;
  padding: 12px;
  background: white;
  border-radius: 6px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: 0.3s;
}

li:hover {
  transform: translateY(-2px);
}

a {
  color: #007bff;
  text-decoration: none;
  font-weight: bold;
}

a:hover {
  text-decoration: underline;
}
</style>
