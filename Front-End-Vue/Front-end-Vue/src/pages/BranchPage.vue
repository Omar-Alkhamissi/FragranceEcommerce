<template>
  <div class="text-center q-mt-md">
    <div class="text-h4">Find 3 Closest Branches To:</div>

    <q-input
      class="q-ma-lg text-h5"
      placeholder="enter current address or postal code"
      v-model="state.address"
      id="address"
    />

    <q-btn
      label="FIND 3"
      @click="findClosestBranches"
      class="q-mb-md"
      style="width: 30vw"
    />

    <div
      ref="mapRef"
      v-show="state.showmap === true"
      style="height: 50vh; width: 90vw; margin-left: 5vw; border: solid"
    ></div>

    <div class="status q-mt-md text-subtitle2 text-negative" v-if="state.status">
      {{ state.status }}
    </div>
  </div>
</template>

<script>
import { ref, reactive, onBeforeUnmount, nextTick } from "vue";

export default {
  name: "BranchPage",
  setup() {
    const mapRef = ref(null);
    let mapObj = null;

    const state = reactive({
      address: "T5A 0A3",
      status: "",
      showmap: false,
    });

    const TOMTOM_KEY = "zXKLKlV5oNoDXSFJUmQuGKA2081UKQ0P";

    const makeMap = (centerLngLat) => {
      if (!window.tt) {
        state.status = "TomTom SDK not loaded on page";
        return null;
      }
      if (mapObj) {
        mapObj.remove();
        mapObj = null;
      }
      mapObj = window.tt.map({
        key: TOMTOM_KEY,
        container: mapRef.value,
        center: centerLngLat,
        zoom: 11,
      });
      if (mapObj && window.tt && typeof window.tt.NavigationControl === "function") {
        mapObj.addControl(new window.tt.NavigationControl());
      }
      return mapObj;
    };

    const addMarker = (lng, lat, html) => {
      const m = new window.tt.Marker().setLngLat([lng, lat]).addTo(mapObj);
      if (html) {
        const popup = new window.tt.Popup({ offset: 25 });
        popup.setHTML(html);
        m.setPopup(popup);
      }
    };

    const geocode = async (query) => {
      const url = `https://api.tomtom.com/search/2/geocode/${encodeURIComponent(
        query
      )}.json?key=${TOMTOM_KEY}&countrySet=CA&typeahead=false&limit=1`;
      const res = await fetch(url);
      if (!res.ok) {
        const msg = await res.text();
        throw new Error(`TomTom geocoding failed: ${res.status} - ${msg}`);
      }
      const json = await res.json();
      const best = json.results?.[0];
      if (!best) throw new Error("No results found for that address");
      return {
        lat: Number(best.position.lat),
        lon: Number(best.position.lon),
        freeform: best.address?.freeformAddress || query,
      };
    };

    const getJwt = () => {
      const direct = sessionStorage.getItem("token") || localStorage.getItem("token");
      if (direct) return direct.replace(/^"|"$/g, "");
      for (const store of [sessionStorage, localStorage]) {
        for (let i = 0; i < store.length; i++) {
          const k = store.key(i);
          const raw = store.getItem(k);
          if (!raw) continue;
          try {
            const obj = JSON.parse(raw);
            if (obj && typeof obj === "object" && typeof obj.token === "string") {
              return obj.token;
            }
          } catch {
            continue;
          }
        }
      }
      return "";
    };

    const fetchBranches = async (lat, lon) => {
      const token = getJwt();
      const res = await fetch(`/api/Branch/${lat}/${lon}`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      });
      if (!res.ok) {
        const msg = await res.text();
        throw new Error(`Branch API failed: ${res.status} - ${msg}`);
      }
      return await res.json();
    };

    const findClosestBranches = async () => {
      state.status = "";
      state.showmap = false;

      if (!state.address?.trim()) {
        state.status = "Enter an address or postal code";
        return;
      }

      try {
        const where = await geocode(state.address.trim());

        state.showmap = true;
        await nextTick();

        const map = makeMap([where.lon, where.lat]);
        if (!map) return;

        const branches = await fetchBranches(where.lat, where.lon);
        if (!branches?.length) {
          state.status = "No branches returned";
          return;
        }

        let bounds = null;
        branches.forEach((b) => {
          const lon = Number(b.longitude ?? b.Longitude);
          const lat = Number(b.latitude ?? b.Latitude);
          const dist = Number(b.distance ?? b.Distance ?? 0);
          if (Number.isFinite(lon) && Number.isFinite(lat)) {
            addMarker(
              lon,
              lat,
              `<div id="popup">Branch #${b.id ?? b.Id}</div>
               <div>${b.street ?? b.Street ?? ""}, ${b.city ?? b.City ?? ""} ${b.region ?? b.Region ?? ""}<br/>
               ${dist.toFixed(2)} mi</div>`
            );
            if (!bounds) {
              bounds = new window.tt.LngLatBounds([lon, lat], [lon, lat]);
            } else {
              bounds.extend([lon, lat]);
            }
          }
        });

        if (bounds) {
          map.fitBounds(bounds, { padding: 80, duration: 500 });
          map.easeTo({ zoom: map.getZoom() - 0.7, duration: 300 });
        }

        state.status = "";
      } catch (err) {
        console.error(err);
        state.status = typeof err?.message === "string" ? err.message : "Unexpected error";
      }
    };

    onBeforeUnmount(() => {
      if (mapObj) {
        mapObj.remove();
        mapObj = null;
      }
    });

    return { mapRef, state, findClosestBranches };
  },
};
</script>

<style scoped>
#popup { font-weight: 600; margin-bottom: 4px; }
.status { white-space: pre-wrap; }
</style>
