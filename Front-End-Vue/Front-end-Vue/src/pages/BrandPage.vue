<template>
  <div class="text-center">
    <!-- Logo -->
    <q-avatar square class="q-mb-md logo-avatar">
      <img src="/img/Spade.png" />
    </q-avatar>

    <div class="text-h2 q-mt-lg">Brands</div>

    <!-- Status message -->
    <div class="status q-mt-md text-subtitle2 text-negative" text-color="red">
      {{ state.status }}
    </div>

    <!-- Brand selector -->
    <q-select
      class="q-mt-lg q-ml-lg"
      v-if="state.brands.length > 0"
      style="width: 50vw; margin-bottom: 4vh; background-color: #fff"
      :option-value="'id'"
      :option-label="'name'"
      :options="state.brands"
      label="Select a Brand"
      v-model="state.selectedBrandId"
      @update:model-value="getProducts"
      emit-value
      map-options
    />

    <!-- Header showing which brand's products -->
    <div class="text-h6 text-bold text-center text-primary" v-if="state.products.length > 0">
      {{ state.selectedBrand.name }} PRODUCTS
    </div>

    <!-- Product list -->
    <q-scroll-area style="height: 55vh">
      <q-list separator bordered class="q-mx-md">
        <q-item
          dense
          clickable
          v-for="product in state.products"
          :key="product.id"
          @click="selectProduct(product.id)"
        >
          <!-- THUMBNAIL -->
          <q-item-section avatar class="thumb-wrapper">
            <q-img
              :src="`/img/${product.graphicName}`"
              class="thumb"
              ratio="1"
              spinner-color="primary"
            />
          </q-item-section>

          <!-- NAME -->
          <q-item-section class="text-left">
            {{ product.productName }}
          </q-item-section>
        </q-item>
      </q-list>

      <!-- Dialog for product details / add-to-cart -->
      <q-dialog v-model="state.itemSelected" transition-show="rotate" transition-hide="rotate">
        <q-card style="min-width: 400px; max-width: 90vw">
          <!-- Close button -->
          <q-card-actions align="right">
            <q-btn flat label="X" color="primary" v-close-popup class="text-h5" />
          </q-card-actions>

          <!-- Product name -->
          <q-card-section>
            <div class="text-subtitle2 text-center">
              {{ state.selectedProduct.productName }}
            </div>
          </q-card-section>

          <!-- Product image -->
          <q-card-section class="row justify-center">
            <q-avatar size="100px">
              <img :src="`/img/${state.selectedProduct.graphicName}`" />
            </q-avatar>
          </q-card-section>

          <!-- MSRP -->
          <q-card-section>
            <div class="text-h6 text-center text-primary">
              {{ formatCurrency(state.selectedProduct.msrp) }}
            </div>
          </q-card-section>

          <!-- Details tooltip -->
          <q-card-section class="text-center">
            <q-chip icon="info" color="primary" text-color="white">
              Details
              <q-tooltip
                transition-show="flip-right"
                transition-hide="flip-left"
                text-color="white"
              >
                <div style="max-width: 260px; text-align: left">
                  {{ state.selectedProduct.description }}
                </div>
              </q-tooltip>
            </q-chip>
          </q-card-section>

          <!-- Improved Qty + Add to cart + View cart section -->
          <q-card-section class="text-center">
            <div class="row justify-center items-start q-col-gutter-md q-mb-md">
              <div class="col-auto">
                <div class="column items-center">
                  <q-input
                    v-model.number="state.qty"
                    type="number"
                    filled
                    dense
                    class="qty-input"
                    input-class="text-center"
                  />
                  <div class="text-caption text-grey-6 q-mt-xs">Qty</div>
                </div>
              </div>

              <div class="col-auto">
                <q-btn
                  class="add-btn"
                  color="primary"
                  label="ADD TO CART"
                  :disable="state.qty <= 0"
                  @click="addToCart"
                  no-caps
                />
              </div>
            </div>

            <q-btn flat label="VIEW CART" @click="viewCart" no-caps color="primary" />
          </q-card-section>

          <!-- Confirmation -->
          <q-card-section class="text-center text-positive">
            {{ state.dialogStatus }}
          </q-card-section>
        </q-card>
      </q-dialog>
    </q-scroll-area>
  </div>
</template>

<script>
import { reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { formatCurrency } from 'src/utils/formatutils.js'
import { fetcher } from 'src/utils/apiutil.js'

export default {
  setup() {
    const router = useRouter()

    const state = reactive({
      status: '',
      brands: [],
      selectedBrandId: '',
      selectedBrand: {},
      products: [],

      // dialog / cart state
      selectedProduct: {},
      itemSelected: false,
      dialogStatus: '',
      cart: [],
      qty: 0,
    })

    /* ----------------- API ----------------- */
    const loadBrands = async () => {
      state.status = 'Loading brands...'
      try {
        const data = await fetcher('Brands')
        if (data.error) {
          state.status = data.error
        } else {
          state.brands = data
          state.status = `Loaded ${state.brands.length} brands`
        }
      } catch (err) {
        console.error(err)
        state.status = `Error: ${err.message}`
      }
    }

    const getProducts = async () => {
      state.selectedBrand = state.brands.find((b) => b.id === state.selectedBrandId)
      if (!state.selectedBrand) return
      state.status = `Finding products for ${state.selectedBrand.name}...`
      try {
        const data = await fetcher(`Products/${state.selectedBrand.id}`)
        if (data.error) {
          state.status = data.error
        } else {
          state.products = data
          state.status = `Loaded ${state.products.length} products for ${state.selectedBrand.name}`
        }
      } catch (err) {
        console.error(err)
        state.status = `Error: ${err.message}`
      }
    }

    /* ------------- Dialog helpers ------------- */
    const selectProduct = (id) => {
      state.selectedProduct = state.products.find((p) => p.id === id) || {}
      state.itemSelected = true
      state.dialogStatus = ''
      state.cart = JSON.parse(sessionStorage.getItem('cart') || '[]')
    }

    /* ------------- Cart logic ------------- */
    const addToCart = () => {
      const idx = state.cart.findIndex((c) => c.id === state.selectedProduct.id)

      if (state.qty > 0) {
        const entry = { id: state.selectedProduct.id, qty: state.qty, item: state.selectedProduct }
        idx === -1 ? state.cart.push(entry) : state.cart.splice(idx, 1, entry)
        state.dialogStatus = `${state.qty} item(s) added`
      } else if (idx !== -1) {
        state.cart.splice(idx, 1)
        state.dialogStatus = 'item(s) removed'
      }

      sessionStorage.setItem('cart', JSON.stringify(state.cart))
      state.qty = 0
    }

    const viewCart = () => router.push('/cart')

    onMounted(loadBrands)

    return {
      state,
      loadBrands,
      getProducts,
      selectProduct,
      addToCart,
      viewCart,
      formatCurrency,
    }
  },
}
</script>

<style scoped>
/* ───────── LOGO SIZING ─────────────────────────────────────────────── */
.logo-avatar {
  width: clamp(90px, 12vw, 180px);
  height: auto;
}

/* ───────── PRODUCT LIST THUMBNAILS ─────────────────────────────────── */
.thumb-wrapper {
  width: 110px;
}

.thumb {
  height: 100%;
  object-fit: contain;
}

/* ───────── IMPROVED BUTTON ALIGNMENT ─────────────────────────────────── */
.qty-input {
  width: 88px;
}

.add-btn {
  height: 40px;
  padding: 0 18px;
}

/* phones */
@media (max-width: 600px) {
  .thumb-wrapper {
    width: 80px;
  }
}
</style>
