<template>
  <v-dialog
    :model-value="modelValue"
    max-width="440"
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <v-card>
      <v-card-title class="text-h6 font-weight-bold pa-5 pb-2">
        Delete Product
      </v-card-title>
      <v-card-text class="px-5 pb-4 text-body-2">
        Are you sure you want to delete
        <strong>{{ product?.name }}</strong>
        (<span class="font-weight-medium">{{ product?.sku }}</span>)?
        This action cannot be undone.
      </v-card-text>
      <v-divider />
      <v-card-actions class="pa-4">
        <v-spacer />
        <v-btn variant="text" :disabled="loading" @click="$emit('update:modelValue', false)">
          Cancel
        </v-btn>
        <v-btn color="error" variant="flat" :loading="loading" @click="$emit('confirm')">
          Delete
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import type { Product } from '@/types/product';

defineProps<{
  modelValue: boolean;
  product: Product | null;
  loading: boolean;
}>();

defineEmits<{
  'update:modelValue': [value: boolean];
  confirm: [];
}>();
</script>
