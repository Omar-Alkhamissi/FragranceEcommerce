<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-toolbar-title>Info3181 Case Study</q-toolbar-title>
         <q-btn flat round dense icon="reorder" class="q-ml-auto">
          <q-menu>
            <q-list style="min-width: 120px">
              <q-item clickable v-close-popup to="/">
                <q-item-section>Home</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/">
                <q-item-section>Brands</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/branches">
                <q-item-section>Branches</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/orderhistory">
                <q-item-section>Order History</q-item-section>
              </q-item>
              <q-item clickable v-close-popup to="/cart">
                <q-item-section>Cart</q-item-section>
              </q-item>
               <q-item clickable v-close-popup to="/register">
                <q-item-section>Register</q-item-section>
              </q-item>
               <q-item clickable v-close-popup to="/login">
                <q-item-section>Login</q-item-section>
              </q-item>
               <q-item clickable v-close-popup to="/logout">
                <q-item-section>Logout</q-item-section>
              </q-item>

            </q-list>
          </q-menu>
        </q-btn>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>

    <!-- Step 15: footer before closing q-layout -->
    <q-footer elevated class="bg-grey-8 text-white">
      <!-- Step 16: toolbar inside footer -->
      <q-toolbar>
        <!-- Step 17: toolbar title -->
        <q-toolbar-title>
          <!-- Step 18: subtitle content -->
          <div class="text-subtitle2">
            Best viewed on mobile device — Omar Alkhamissi &copy;{{ new Date().getFullYear() }}
          </div>
        </q-toolbar-title>
      </q-toolbar>
    </q-footer>
  </q-layout>
</template>

<script>
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'MainLayout',
  setup() {
    return {}
  },
})
</script>
