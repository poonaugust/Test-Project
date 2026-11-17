<template>
  <v-app>
    <v-navigation-drawer v-model="drawer" app width="250">
      <v-list-item
        class="py-4"
        prepend-icon="mdi-layers-triple"
        title="TestProject"
        subtitle="Clean Architecture"
      />
      <v-divider />

      <v-list nav density="comfortable" class="mt-2">
        <v-list-item
          to="/"
          exact
          prepend-icon="mdi-view-dashboard-outline"
          title="Dashboard"
          color="primary"
        />
        <v-list-item
          to="/products"
          prepend-icon="mdi-package-variant-closed"
          title="Products"
          color="primary"
        />
      </v-list>

      <template #append>
        <v-divider />
        <v-list nav density="compact">
          <v-list-item
            href="/swagger"
            target="_blank"
            prepend-icon="mdi-api"
            title="Swagger API Docs"
            append-icon="mdi-open-in-new"
          />
        </v-list>
      </template>
    </v-navigation-drawer>

    <v-app-bar app flat border="b">
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-app-bar-title class="font-weight-medium">
        {{ route.meta.title || 'Workspace' }}
      </v-app-bar-title>

      <v-spacer />

      <v-btn
        icon
        variant="text"
        :title="isDark ? 'Switch to Light Mode' : 'Switch to Dark Mode'"
        @click="toggleTheme"
      >
        <v-icon>{{ isDark ? 'mdi-weather-sunny' : 'mdi-weather-night' }}</v-icon>
      </v-btn>
    </v-app-bar>

    <v-main>
      <v-container fluid class="pa-6">
        <slot />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import { useTheme } from 'vuetify';

const drawer = ref(true);
const route = useRoute();
const theme = useTheme();

const isDark = computed(() => theme.global.current.value.dark);

function toggleTheme() {
  theme.global.name.value = isDark.value ? 'light' : 'dark';
}
</script>
