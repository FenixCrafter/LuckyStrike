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
            isLoggedIn: false,
        }
    },
    mutations: {
        // login mutation to set the user state
        logIn(state: RootState, token: string) {
            state.user.isLoggedIn = true;
            state.user.token = token;
            const data = getPayloadFromToken(token);
            state.user.username = data.UserName;
            state.user.userId = data.UserId;
            state.user.refreshToken = data.RefreshToken;
        },
        // logout mutation to clear the user state
        logOut(state: RootState) {
            localStorage.removeItem("Token");
            state.user.isLoggedIn = false;
            state.user.token = '';
            state.user.username = '';
            state.user.userId = '';
        },
    },
    // getters to get the user state
    getters: {
        getUser: (state: RootState) => state.user,
        getIsLoggedIn: (state:RootState) => state.user.isLoggedIn,
    },
    // plugins to persist the user state (no wipe when refresh)
    plugins: [createPersistedState()]
});

export function useStore () {
    return baseUseStore(key)
}

export default store;
