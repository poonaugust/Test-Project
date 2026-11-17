<template>
  <v-dialog
    :model-value="modelValue"
    max-width="600"
    persistent
    @update:model-value="$emit('update:modelValue', $event)"
  >
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between pa-5 pb-3">
        <span class="text-h6 font-weight-bold">
          {{ isEdit ? 'Edit Product' : 'New Product' }}
        </span>
        <v-btn icon="mdi-close" variant="text" size="small" @click="close" />
      </v-card-title>

      <v-divider />

      <v-form ref="formRef" @submit.prevent="submit">
        <v-card-text class="pa-5">
          <v-alert
            v-if="errorMessage"
            type="error"
            variant="tonal"
            class="mb-4"
            closable
            @click:close="errorMessage = null"
          >
            {{ errorMessage }}
          </v-alert>

          <v-row dense>
            <v-col cols="12" sm="5">
              <v-text-field
                v-model="form.sku"
                label="SKU *"
                placeholder="TECH-001"
                :rules="[requiredRule, maxLengthRule(50)]"
              />
            </v-col>
            <v-col cols="12" sm="7">
              <v-text-field
                v-model="form.category"
                label="Category *"
                placeholder="Peripherals"
                :rules="[requiredRule, maxLengthRule(100)]"
              />
            </v-col>

            <v-col cols="12">
              <v-text-field
                v-model="form.name"
                label="Product Name *"
                placeholder="Mechanical Keyboard Pro"
                :rules="[requiredRule, maxLengthRule(200)]"
              />
            </v-col>

            <v-col cols="12" sm="4">
              <v-text-field
                v-model.number="form.price"
                label="Price *"
                type="number"
                step="0.01"
                min="0"
                :rules="[nonNegativeRule]"
              />
            </v-col>
            <v-col cols="12" sm="4">
              <v-text-field
                v-model.number="form.stockQuantity"
                label="Stock *"
                type="number"
                min="0"
                :rules="[nonNegativeRule]"
              />
            </v-col>
            <v-col cols="12" sm="4">
              <v-select
                v-model="form.status"
                label="Status *"
                :items="statusOptions"
              />
            </v-col>

            <v-col cols="12">
              <v-textarea
                v-model="form.description"
                label="Description"
                rows="3"
                auto-grow
                :rules="[maxLengthOptionalRule(1000)]"
              />
            </v-col>
          </v-row>
        </v-card-text>

        <v-divider />

        <v-card-actions class="pa-4">
          <v-spacer />
          <v-btn variant="text" :disabled="saving" @click="close">Cancel</v-btn>
          <v-btn color="primary" variant="flat" type="submit" :loading="saving">
            {{ isEdit ? 'Save Changes' : 'Create Product' }}
          </v-btn>
        </v-card-actions>
      </v-form>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue';
import type { Product, ProductStatus, UpsertProductPayload } from '@/types/product';

const props = defineProps<{
  modelValue: boolean;
  product: Product | null;
  saving: boolean;
}>();

const emit = defineEmits<{
  'update:modelValue': [value: boolean];
  save: [payload: UpsertProductPayload];
}>();

const formRef = ref<{ validate: () => Promise<{ valid: boolean }> } | null>(null);
const errorMessage = ref<string | null>(null);

const statusOptions: ProductStatus[] = ['Active', 'Draft', 'Archived'];

const form = reactive<UpsertProductPayload>({
  sku: '',
  name: '',
  description: '',
  category: '',
  price: 0,
  stockQuantity: 0,
  status: 'Active',
});

const isEdit = computed(() => Boolean(props.product));

watch(
  () => [props.modelValue, props.product] as const,
  ([open, product]) => {
    if (!open) return;
    errorMessage.value = null;
    if (product) {
      form.sku = product.sku;
      form.name = product.name;
      form.description = product.description ?? '';
      form.category = product.category;
      form.price = product.price;
      form.stockQuantity = product.stockQuantity;
      form.status = product.status;
    } else {
      form.sku = '';
      form.name = '';
      form.description = '';
      form.category = '';
      form.price = 0;
      form.stockQuantity = 0;
      form.status = 'Active';
    }
  },
  { immediate: true },
);

const requiredRule = (v: string) => Boolean(v?.trim()) || 'This field is required.';
const maxLengthRule = (max: number) => (v: string) =>
  !v || v.length <= max || `Must not exceed ${max} characters.`;
const maxLengthOptionalRule = (max: number) => (v: string | null) =>
  !v || v.length <= max || `Must not exceed ${max} characters.`;
const nonNegativeRule = (v: number) => v >= 0 || 'Value cannot be negative.';

function setError(message: string) {
  errorMessage.value = message;
}

defineExpose({ setError });

function close() {
  emit('update:modelValue', false);
}

async function submit() {
  const result = await formRef.value?.validate();
  if (!result?.valid) return;

  emit('save', {
    sku: form.sku.trim(),
    name: form.name.trim(),
    description: form.description?.trim() || null,
    category: form.category.trim(),
    price: Number(form.price),
    stockQuantity: Number(form.stockQuantity),
    status: form.status,
  });
}
</script>
