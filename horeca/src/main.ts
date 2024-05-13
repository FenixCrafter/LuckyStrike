import './assets/css/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import store, {key} from './store/store';
import router from './router'
import ToastPlugin from "vue-toast-notification";

import axios from "axios";

import 'vue-toast-notification/dist/theme-sugar.css';

axios.defaults.baseURL = "https://localhost:7253";

const app = createApp(App)

app.config.globalProperties.http = axios.create()

app.config.globalProperties.http.interceptors.request.use(function (config: any) {
    const token = localStorage.getItem("Token");
    if (token != null && token != "")
    {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  }, function (error) {
    // Do something with request error
    return Promise.reject(error);
});

app.use(store, key)
app.use(router)
app.use(ToastPlugin)

app.mount('#app')
