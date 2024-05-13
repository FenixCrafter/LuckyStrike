import './assets/css/main.css'
import './assets/css/colors.scss'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import {store, key, useStore} from './store/store';
import ToastPlugin from "vue-toast-notification";

//packages
import VueDatePicker from '@vuepic/vue-datepicker';
import axios from "axios";

//css
import '@vuepic/vue-datepicker/dist/main.css'
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

app.config.globalProperties.http.interceptors.response.use(function (response: any) {
    return response;
  }, function (error) {
    if (error.message == "Network Error") {
      return Promise.reject({ response: "Kan geen verbinding maken met LuckyStrike. Probeer het later opnieuw" });
    }
    return Promise.reject(error);
});

app.component('VueDatePicker', VueDatePicker)

app.use(router)
app.use(store, key)
app.use(ToastPlugin)

app.mount('#app')

