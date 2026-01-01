<template>
  <div class="text-center">
    <div class="text-h4 q-mt-md text-primary">Cart Contents</div>

    <q-icon name="shopping_cart" size="64px" class="q-mt-md" />

    <div
      :class="[state.cart.length ? 'text-positive' : 'text-primary', 'q-mt-sm', 'text-subtitle2']"
    >
      {{ state.status }}
    </div>

    <q-card class="q-ma-md" v-if="state.cart.length">
      <q-table
        flat
        dense
        hide-bottom
        :pagination="{ rowsPerPage: 0 }"
        :rows="itemRows"
        :columns="itemColumns"
        row-key="name"
      >
        <template #body-cell-msrp="props">
          <q-td align="right">{{ formatCurrency(props.row.msrp) }}</q-td>
        </template>
        <template #body-cell-extended="props">
          <q-td align="right">{{ formatCurrency(props.row.extended) }}</q-td>
        </template>
      </q-table>

      <q-table
        flat
        dense
        hide-header
        hide-bottom
        :pagination="{ rowsPerPage: 0 }"
        :rows="totalRows"
        :columns="totalColumns"
        row-key="label"
      >
        <template #body-cell-label="props">
          <q-td class="text-right text-bold">{{ props.row.label }}</q-td>
        </template>
        <template #body-cell-value="props">
          <q-td class="text-right">{{ formatCurrency(props.row.value) }}</q-td>
        </template>
      </q-table>
    </q-card>

    <div v-else class="text-primary q-mt-md">cart emptied</div>

    <div class="q-mt-lg" v-if="state.cart.length">
      <q-btn
        class="q-mr-sm"
        color="primary"
        label="Checkout"
        :disable="state.cart.length < 1"
        @click="checkout()"
      />
      <q-btn color="negative" label="Empty Cart" @click="clearCart" />
    </div>

    <!-- Show Order History button after successful order placement -->
    <div v-if="state.orderPlaced" class="q-mt-lg">
      <q-btn
        color="positive"
        label="View Order History"
        icon="history"
        @click="goToOrderHistory"
        class="q-mb-md"
      />
      <div class="text-caption text-grey-6">
        Your order has been placed successfully!
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { formatCurrency } from 'src/utils/formatutils'
import { poster } from 'src/utils/apiutil.js'

const TAX_RATE = 0.13
const router = useRouter()

const itemColumns = [
  { name: 'name', label: 'Name', field: 'name', align: 'left' },
  { name: 'qty', label: 'Qty', field: 'qty', align: 'right' },
  { name: 'msrp', label: 'MSRP', field: 'msrp', align: 'right' },
  { name: 'extended', label: 'Extended', field: 'extended', align: 'right' },
]

const totalColumns = [
  { name: 'label', field: 'label', align: 'right' },
  { name: 'value', field: 'value', align: 'right' },
]

const state = reactive({
  status: '',
  cart: [],
  sub: 0,
  tax: 0,
  total: 0,
  orderPlaced: false, // Track if order was successfully placed
})

// Fixed: Ensure proper rounding to 2 decimal places for all currency calculations
const roundToTwoDecimals = (num) => {
  return Math.round((parseFloat(num) || 0) * 100) / 100
}

const itemRows = computed(() =>
  state.cart.map(({ qty, item }) => {
    const quantity = parseInt(qty) || 0
    const price = roundToTwoDecimals(item.msrp)
    const extended = roundToTwoDecimals(quantity * price)

    return {
      name: item.productName,
      qty: quantity,
      msrp: price,
      extended: extended,
    }
  }),
)

const totalRows = computed(() => [
  { label: 'Sub:', value: state.sub },
  { label: `Tax(${TAX_RATE * 100}%):`, value: state.tax },
  { label: 'Total:', value: state.total },
])

function recalcTotals() {
  let subtotal = 0

  state.cart.forEach(cartItem => {
    const qty = parseInt(cartItem.qty) || 0
    const price = roundToTwoDecimals(cartItem.item.msrp)
    subtotal += (qty * price)
  })
  state.sub = roundToTwoDecimals(subtotal)
  state.tax = roundToTwoDecimals(state.sub * TAX_RATE)
  state.total = roundToTwoDecimals(state.sub + state.tax)
}

function loadCart() {
  state.cart = JSON.parse(sessionStorage.getItem('cart') || '[]')
  state.status = state.cart.length ? `Loaded ${state.cart.length} product(s)` : 'cart emptied'
  state.orderPlaced = false
  recalcTotals()
}

function clearCart(keepStatus = false) {
  sessionStorage.removeItem('cart')
  state.cart.length = 0
  state.orderPlaced = false
  recalcTotals()
  if (!keepStatus) {
    state.status = 'cart emptied'
  }
}

const checkout = async () => {
  const customer = JSON.parse(sessionStorage.getItem('customer'))
  const cart = JSON.parse(sessionStorage.getItem('cart'))

  if (!customer?.email) {
    state.status = 'Please login first'
    return
  }

  if (!cart?.length) {
    state.status = 'Cart is empty'
    return
  }

  try {
    state.status = 'sending order info to server'

    const selections = cart.map(cartItem => ({
      Qty: parseInt(cartItem.qty) || 1,
      Item: {
        Id: cartItem.item.id,
        BrandId: parseInt(cartItem.item.brandId) || 0,
        ProductName: cartItem.item.productName || '',
        GraphicName: cartItem.item.graphicName || '',
        CostPrice: roundToTwoDecimals(cartItem.item.costPrice || cartItem.item.msrp || 0),
        MSRP: roundToTwoDecimals(cartItem.item.msrp || 0),
        QtyOnHand: parseInt(cartItem.item.qtyOnHand || 999),
        QtyOnBackOrder: parseInt(cartItem.item.qtyOnBackOrder || 0),
        Description: cartItem.item.description || ''
      }
    }))

    const orderHelper = {
      Email: customer.email,
      Selections: selections
    }

    const payload = await poster('order', orderHelper)

    if (payload?.includes('saved')) {
      const orderMatch = payload.match(/Order (\d+) saved/i)
      const orderId = orderMatch?.[1]
      state.status = orderId ? `Order #${orderId} Placed!` : 'Order Placed Successfully!'
      clearCart(true)
      state.orderPlaced = true
    } else {
      state.status = payload || 'Order failed'
    }
  } catch (err) {
    state.status = `Error placing order: ${err.message}`
  }
}

const goToOrderHistory = () => {
  router.push('/orderhistory')
}

onMounted(loadCart)
</script>
