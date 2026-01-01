<template>
  <div class="text-center">
    <div class="text-h4 q-mt-md text-primary">Order History</div>
    <div class="text-h6 text-negative q-mt-sm">{{ state.status }}</div>
    <div class="q-mt-lg q-mx-md">
      <table class="full-width order-table">
        <thead>
          <tr class="table-header">
            <th class="text-center">#</th>
            <th class="text-center">Date</th>
            <th class="text-center">Total</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(order, index) in state.orders"
            :key="order.id"
            class="table-row cursor-pointer"
            :class="{ 'even-row': index % 2 === 0 }"
            @click="selectOrder(order.id)"
          >
            <td class="text-center">{{ order.id }}</td>
            <td class="text-center">{{ formatDate(order.orderDate) }}</td>
            <td class="text-center">{{ formatCurrency(order.orderAmount) }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Order Details Dialog -->
    <q-dialog v-model="state.orderSelected" transition-show="rotate" transition-hide="rotate">
      <q-card style="min-width:350px">
        <q-card-actions align="right">
          <q-btn flat label="X" color="primary" v-close-popup class="text-h5" />
        </q-card-actions>

        <q-card-section class="text-center">
          <div class="text-h5 text-primary text-weight-bold">
            Order #{{ state.selectedOrderId }}
          </div>
          <div class="text-body2 q-mt-xs">
            {{ state.selectedOrderDate }}
          </div>
          <q-icon name="shopping_cart" size="56px" color="primary" />
        </q-card-section>

        <q-card-section class="q-pt-none">
          <table class="full-width details-table">
            <thead>
              <tr class="details-header">
                <th class="text-left">Name</th>
                <th class="text-center" colspan="3">Quantities</th>
                <th class="text-right">Extended</th>
              </tr>
              <tr class="sub-header">
                <th></th>
                <th class="text-center">S</th>
                <th class="text-center">O</th>
                <th class="text-center">B</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in state.orderDetails" :key="item.productId" class="details-row">
                <td class="text-left">{{ item.productName }}</td>
                <td class="text-center">{{ item.qtySold }}</td>
                <td class="text-center">{{ item.qtyOrdered }}</td>
                <td class="text-center">{{ item.qtyBackOrdered }}</td>
                <td class="text-right">{{ calculateExtended(item) }}</td>
              </tr>
            </tbody>
          </table>
        </q-card-section>

        <!-- Totals Section -->
        <q-card-section class="q-pa-sm q-pt-md">
          <table class="full-width totals-table">
            <tbody>
              <tr>
                <td class="text-right text-weight-bold">Sub:</td>
                <td class="text-right">{{ formatCurrency(state.orderTotals.subTotal) }}</td>
              </tr>
              <tr>
                <td class="text-right text-weight-bold">Tax(13%):</td>
                <td class="text-right">{{ formatCurrency(state.orderTotals.tax) }}</td>
              </tr>
              <tr>
                <td class="text-right text-weight-bold">Total:</td>
                <td class="text-right text-primary text-weight-bold">
                  {{ formatCurrency(state.orderTotals.total) }}
                </td>
              </tr>
            </tbody>
          </table>
        </q-card-section>

        <q-card-section class="text-center q-pt-sm">
          <div class="text-caption text-positive text-weight-bold">
            Details for order {{ state.selectedOrderId }}
          </div>
        </q-card-section>
      </q-card>
    </q-dialog>
  </div>
</template>

<script>
import { reactive, onMounted } from 'vue'
import { fetcher } from 'src/utils/apiutil.js'
import { formatCurrency, formatLocalDateTime } from 'src/utils/formatutils.js'

const formatDate = (date) => {
  const d = new Date(Date.parse(date))
  const y = d.getFullYear()
  const m = (d.getMonth() + 1).toString().padStart(2, '0')
  const day = d.getDate().toString().padStart(2, '0')
  return `${y}-${m}-${day}`
}

const roundToTwoDecimals = (num) => {
  return Math.round((parseFloat(num) || 0) * 100) / 100
}

export default {
  name: 'OrderHistoryPage',
  setup () {
    const TAX_RATE = 0.13

    const state = reactive({
      status: '',
      orders: [],
      orderSelected: false,
      selectedOrderId: 0,
      selectedOrderDate: '',
      orderDetails: [],
      orderTotals: { subTotal: 0, tax: 0, total: 0 }
    })

    const calculateExtended = (item) => {
      const qty = parseFloat(item.qtyOrdered) || 0
      const price = parseFloat(item.sellingPrice) || 0
      const result = roundToTwoDecimals(qty * price)
      return formatCurrency(result)
    }

    const getExtendedAmount = (item) => {
      const qty = Number(item.qtyOrdered) || 0
      const price = Number(item.sellingPrice) || 0
      return roundToTwoDecimals(qty * price)
    }

    const recalcTotals = () => {
      let subtotal = 0

      state.orderDetails.forEach(item => {
        subtotal += getExtendedAmount(item)
      })

      subtotal = roundToTwoDecimals(subtotal)
      const tax = roundToTwoDecimals(subtotal * TAX_RATE)
      const total = roundToTwoDecimals(subtotal + tax)

      state.orderTotals = { subTotal: subtotal, tax, total }
    }

    const loadOrders = async () => {
      state.status = 'Loading orders...'
      try {
        const customer = JSON.parse(sessionStorage.getItem('customer'))
        const data = await fetcher(`order/${customer.email}`)
        if (data?.error) {
          state.status = data.error
        } else {
          state.orders = data || []
          state.status = `loaded ${state.orders.length} orders`
        }
      } catch (err) {
        console.error(err)
        state.status = 'Could not load orders'
      }
    }

    const selectOrder = async (orderId) => {
      state.selectedOrderId = orderId
      state.status = 'Loading order details...'
      try {
        const customer = JSON.parse(sessionStorage.getItem('customer'))
        const payload = await fetcher(`order/${orderId}/${customer.email}`)
        if (payload?.error) {
          state.status = payload.error
          return
        }
        state.orderDetails = payload || []
        if (state.orderDetails.length > 0) {
          state.selectedOrderDate = formatLocalDateTime(state.orderDetails[0].dateCreated)
        }

        recalcTotals()
        state.orderSelected = true
        state.status = ''
      } catch (err) {
        console.error(err)
        state.status = 'Failed to load order details'
      }
    }

    onMounted(loadOrders)

    return {
      state,
      selectOrder,
      formatDate,
      formatCurrency,
      calculateExtended
    }
  }
}
</script>

<style scoped>
/* Table styles that maintain your existing design */
.order-table {
  border-collapse: collapse;
  border: 1px solid #dee2e6;
}

.table-header {
  background-color: #f8f9fa;
  border-bottom: 2px solid #dee2e6;
}

.table-header th {
  padding: 8px;
  font-weight: bold;
  color: #1976d2;
}

.table-row {
  border-bottom: 1px solid #dee2e6;
}

.table-row td {
  padding: 8px;
}

.even-row {
  background-color: #f8f9fa;
}

.table-row:hover {
  background-color: #e3f2fd;
}

.details-table {
  border-collapse: collapse;
  width: 100%;
}

.details-header {
  border-bottom: 2px solid #e6e6e6;
}

.details-header th {
  padding: 4px 8px;
  font-weight: bold;
  color: #1976d2;
}

.sub-header {
  background-color: #f0f0f0;
  border-bottom: 1px solid #dee2e6;
}

.sub-header th {
  padding: 4px 8px;
  font-size: 12px;
  color: #1976d2;
}

.details-row {
  border-bottom: 1px solid #f0f0f0;
}

.details-row td {
  padding: 8px;
}

.totals-table {
  border-collapse: collapse;
  width: 100%;
}

.totals-table td {
  padding: 4px 8px;
  width: 50%;
}

.cursor-pointer {
  cursor: pointer;
}

.full-width {
  width: 100%;
}
</style>
