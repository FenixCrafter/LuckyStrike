// @ts-ignore
import {createStore, Store,  useStore as baseUseStore,} from 'vuex';
import type { InjectionKey } from 'vue'
import createPersistedState from "vuex-persistedstate";
import type { User } from './types';
import { getPayloadFromToken } from './auth-functions';

interface RootState {
    user: User;
}
export const key: InjectionKey<Store<RootState>> = Symbol()

export const store = createStore<RootState>({
    state: {
        user: {
            username: '',
            userId: '',
            token: '',
            refreshToken: '',
            isLoggedIn: true,
        }
    },
    mutations: {
        logIn(state: RootState, token: string) {
            state.user.isLoggedIn = true;
            state.user.token = token;
            const data = getPayloadFromToken(token);
            state.user.username = data.UserName;
            state.user.userId = data.UserId;
            state.user.refreshToken = data.RefreshToken;
        },
        logOut(state: RootState) {
            localStorage.removeItem("Token");
            localStorage.removeItem("Code");
            state.user.isLoggedIn = false;
            state.user.token = '';
            state.user.username = '';
            state.user.userId = '';
        },
    },
    getters: {
        getUserName: (state: RootState) => state.user.username,
        getUser: (state: RootState) => state.user,
        getIsLoggedIn: (state:RootState) => state.user.isLoggedIn,
    },
    plugins: [createPersistedState()]
});

export function useStore () {
    return baseUseStore(key)
}

export default store;
