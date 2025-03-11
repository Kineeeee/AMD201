<script setup>
import { ref, computed } from 'vue';
import axios from 'axios';

const API_URL = 'http://localhost:5043'; // Cấu hình API backend

const originalUrl = ref('');
const shortUrl = ref(null);
const fullShortUrl = ref(null);
const error = ref(null);
const successMessage = ref(null);
const copied = ref(false);
const isLoading = ref(false);

// ✅ Lấy domain của hệ thống để kiểm tra URL rút gọn
const baseUrl = 'http://localhost:5043';

const isValidUrl = (url) => {
  const pattern = /^(https?:\/\/)((localhost|\d{1,3}(\.\d{1,3}){3})|([\w-]+(\.[a-zA-Z]{2,})+))(:\d{1,5})?(\/.*)?$/;
  return pattern.test(url);
};

const isShortenedUrl = (url) => {
  // Kiểm tra nếu URL bắt đầu với "http://localhost:5043/" và có chứa một shortUrl hợp lệ
  const regex = /^http:\/\/localhost:5043\/[a-zA-Z0-9_-]+$/;
  return regex.test(url);
};


const shortenUrl = async () => {
  if (!isValidUrl(originalUrl.value)) {
    error.value = "❌ URL không hợp lệ! Vui lòng nhập đúng định dạng.";
    return;
  }

  if (isShortenedUrl(originalUrl.value)) {
    error.value = "⚠ URL này đã được rút gọn trước đó!";
    return;
  }

  try {
    error.value = null;
    successMessage.value = null;
    copied.value = false;
    isLoading.value = true;

    const response = await axios.post(`${API_URL}/shorten`, originalUrl.value, {
      headers: { 'Content-Type': 'application/json' }
    });

    shortUrl.value = response.data.shortUrl;
    fullShortUrl.value = response.data.fullShortUrl;
    successMessage.value = "✅ Rút gọn URL thành công!";
    originalUrl.value = "";
  } catch (err) {
    error.value = err.response?.data?.message || "⚠ Không thể rút gọn URL!";
    console.error(err);
  } finally {
    isLoading.value = false;
  }
};

const copyToClipboard = async () => {
  if (fullShortUrl.value) {
    try {
      await navigator.clipboard.writeText(fullShortUrl.value);
      copied.value = true;
      setTimeout(() => copied.value = false, 2000);
    } catch (err) {
      console.error("⚠ Lỗi sao chép URL:", err);
    }
  }
};

// Computed để kiểm tra trạng thái nút
const isButtonDisabled = computed(() => !originalUrl.value || isLoading.value);
</script>


<template>
  <div class="url-form">
    <h2>Nhập URL để rút gọn</h2>

    <div class="input-group">
      <input 
        v-model="originalUrl" 
        placeholder="Nhập URL (bắt buộc có http/https)" 
        @keyup.enter="shortenUrl"
        :disabled="isLoading"
      />
      <button @click="shortenUrl" :disabled="isButtonDisabled">
        <span v-if="isLoading">⏳ Đang xử lý...</span>
        <span v-else>Rút gọn</span>
      </button>
    </div>

    <div v-if="fullShortUrl" class="result">
      <p>
        URL rút gọn: 
        <a :href="fullShortUrl" target="_blank">{{ fullShortUrl }}</a>
      </p>
      <button @click="copyToClipboard">📋 Sao chép</button>
      <span v-if="copied" class="copied-text">✔ Đã sao chép!</span>
    </div>

    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="successMessage" class="success">{{ successMessage }}</p>
  </div>
</template>

<style scoped>
.url-form {
  text-align: center;
  margin: 20px auto;
  max-width: 500px;
  padding: 20px;
  background: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

h2 {
  color: #333;
}

.input-group {
  display: flex;
  gap: 10px;
  justify-content: center;
}

input {
  padding: 10px;
  width: 70%;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-size: 16px;
  transition: 0.3s;
}

input:disabled {
  background: #eee;
  cursor: not-allowed;
}

button {
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

.result {
  margin-top: 15px;
  text-align: center;
}

.result p {
  font-size: 18px;
  font-weight: bold;
}

.result a {
  color: #007bff;
  text-decoration: none;
}

.result a:hover {
  text-decoration: underline;
}

.copied-text {
  display: inline-block;
  margin-left: 10px;
  color: green;
  font-weight: bold;
  font-size: 14px;
}

.error {
  color: red;
  font-size: 14px;
  margin-top: 10px;
  font-weight: bold;
}

.success {
  color: green;
  font-size: 14px;
  margin-top: 10px;
  font-weight: bold;
}
</style>
