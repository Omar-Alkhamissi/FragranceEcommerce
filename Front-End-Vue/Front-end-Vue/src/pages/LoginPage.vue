<template>
  <div class="text-center">
    <!-- Logo -->
    <q-avatar square class="q-mb-md logo-avatar">
      <img src="/img/Spade.png" />
    </q-avatar>
  </div>
  <div class="text-h4 text-center q-mt-md q-mb-md text-primary">Login</div>
  <div
    class="text-title2 text-center text-bold q-mt-sm"
    :class="state.status.includes('successful') ? 'text-positive' : 'text-negative'"
  >
    {{ state.status }}
  </div>
  <q-card class="q-ma-md q-pa-md">
    <q-form ref="myForm" class="q-gutter-md" greedy @submit="login" @reset="resetFields">
      <q-input
        outlined
        placeholder="Email"
        id="email"
        v-model="state.email"
        :rules="[isRequired, isValidEmail]"
      />
      <q-input
        outlined
        placeholder="Password"
        id="password"
        v-model="state.password"
        type="password"
        :rules="[isRequired]"
      />
      <q-btn label="Login" type="submit" />
      <q-btn label="Reset" type="reset" />
    </q-form>
  </q-card>
</template>

<script>
import { reactive } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { poster } from 'src/utils/apiutil.js'

export default {
  setup() {
    const router = useRouter()
    const route = useRoute()

    let state = reactive({
      status: '',
      email: '',
      password: '',
    })

    const isRequired = (val) => {
      return !!val || 'field is required'
    }

    const isValidEmail = (val) => {
      const emailPattern =
        /^(?=[a-zA-Z0-9@._%+-]{6,254}$)[a-zA-Z0-9._%+-]{1,64}@(?:[a-zA-Z0-9-]{1,63}\.){1,8}[a-zA-Z]{2,63}$/
      return emailPattern.test(val) || 'Invalid email'
    }

    const login = async () => {
      state.status = 'logging in with server'

      let customerHelper = {
        email: state.email,
        password: state.password,
      }

      try {
        let payload = await poster('customer/login', customerHelper)
        console.log('Full payload received from server:', payload) // DEBUG LINE
        // Check for any failure messages
        if (
          payload.token &&
          !payload.token.includes('invalid') &&
          !payload.token.includes('failed')
        ) {
          sessionStorage.setItem('customer', JSON.stringify(payload))
          console.log('Stored in sessionStorage:', JSON.stringify(payload)) // DEBUG LINE
          state.status = 'login successful'
          route.query.nextUrl
            ? router.push({ path: route.query.nextUrl })
            : router.push({ path: '/' })
        } else {
          state.status = payload.token
        }
      } catch (err) {
        state.status = err.message
      }
    }

    const resetFields = () => {
      state.email = ''
      state.password = ''
      state.status = ''
    }

    return {
      state,
      login,
      isRequired,
      isValidEmail,
      resetFields,
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

/* phones */
@media (max-width: 600px) {
  .thumb-wrapper {
    width: 80px;
  }
}
</style>
