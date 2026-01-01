<template>
  <div class="text-h4 text-center q-mt-md q-mb-md text-primary">Register</div>
  <div class="text-title2 text-center text-positive text-bold q-mt-sm">{{ state.status }}</div>
  <q-card class="q-ma-md q-pa-md">
    <q-form ref="myForm" class="q-gutter-md" greedy @submit="register" @reset="resetFields">
      <q-input
        outlined
        placeholder="First Name"
        id="firstname"
        v-model="state.firstname"
        :rules="[isRequired]"
      />
      <q-input
        outlined
        placeholder="Last Name"
        id="lastname"
        v-model="state.lastname"
        :rules="[isRequired]"
      />
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
      <q-btn label="Register" type="submit" />
      <q-btn label="Reset" type="reset" />
    </q-form>
  </q-card>
</template>

<script>
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { poster } from 'src/utils/apiutil.js'

export default {
  setup() {
    const router = useRouter()

    let state = reactive({
      status: '',
      firstname: '',
      lastname: '',
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

    const register = async () => {
      state.status = 'registering with server'

      let customerHelper = {
        firstname: state.firstname,
        lastname: state.lastname,
        email: state.email,
        password: state.password,
      }

      try {
        let payload = await poster('customer/register', customerHelper)
        state.status = payload.token

        if (payload.token === "customer registered") {
          setTimeout(() => {
            router.push('/login')
          }, 1200)
        }
      } catch (err) {
        state.status = err.message
      }
    }

    const resetFields = () => {
      state.firstname = ''
      state.lastname = ''
      state.email = ''
      state.password = ''
      state.status = ''
    }

    return {
      state,
      register,
      isRequired,
      isValidEmail,
      resetFields,
    }
  },
}
</script>
