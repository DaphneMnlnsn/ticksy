<template>
  <div v-if="modelValue" class="overlay">

    <div class="overlay-bg" @click="$emit('update:modelValue', false)"></div>

    <div class="side-panel">

      <div class="modal-header">
        <h2>Edit Member</h2>
        <button class="close-btn" @click="$emit('update:modelValue', false)">✕</button>
      </div>

      <div class="edit-profile">
        <img :src="avatar" class="edit-avatar" />
        <div>
          <div class="edit-name">{{ localUser?.name }}</div>
          <div class="edit-sub">Member since Dec 1994</div>
        </div>
      </div>

      
      <div class="section">
        <p class="section-title">Member Details</p>

        <div class="form-group">
          <label>Name</label>
          <input class="input" v-model="localUser.name" />
        </div>

        <div class="form-group">
          <label>Email</label>
          <input class="input" v-model="localUser.email" />
        </div>

        <div class="form-group">
          <label>Contact Number</label>
          <input class="input" />
        </div>
      </div>

<div class="section">
  <p class="section-title">Assignments</p>

  <div class="form-group">
    <label>Assign to Team</label>
    <select class="input" v-model="localUser.team">
      <option value="Dev Team">Dev Team</option>
      <option value="QA Team">QA Team</option>
    </select>
  </div>

  <div class="form-group">
    <label>Assign to Schedule</label>
    <select class="input" v-model="localUser.schedule">
      <option value="Hybrid Schedule">Hybrid Schedule</option>
      <option value="Onsite">Onsite</option>
    </select>
  </div>
</div>

      <div class="modal-footer">
        <button class="cancel" @click="$emit('update:modelValue', false)">Cancel</button>
        <button class="save" @click="save">Save</button>
      </div>

    </div>
  </div>
</template>

<script setup>
  import { ref, watch } from 'vue'

  const props = defineProps({
    modelValue: Boolean,
    user: Object,
    avatar: String
  })

  const emit = defineEmits(['update:modelValue', 'save'])

  const localUser = ref({})

  watch(() => props.user, (val) => {
    localUser.value = val ? { ...val } : {}
  }, { immediate: true })

  function save() {
    emit('save', localUser.value)
    emit('update:modelValue', false)
  }
</script>

<style scoped>

  .overlay {
    position: fixed;
    inset: 0;
    z-index: 999;
    display: flex;
    justify-content: flex-end;
  }

  .overlay-bg {
    position: absolute;
    inset: 0;
    background: rgba(0, 19, 36, 0.4);
  }

  .side-panel {
    position: relative;
    width: 400px;
    height: 100vh;
    background: #031e2f;
    padding: 20px;
    color: white;
    overflow-y: auto;
    box-shadow: -5px 0 20px rgba(0,0,0,0.5);
    animation: slideIn 0.3s ease;
  }

  @keyframes slideIn {
    from { transform: translateX(100%); }
    to { transform: translateX(0); }
  }

  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px 20px;
    margin: -20px -20px 0 -20px; /* 👈 ito mag aangat */
    padding-left: 20px;
    padding-right: 20px;
    border-bottom: 1px solid rgba(255,255,255,0.1);
  }

  .edit-profile {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 25px 0;
    margin: 0 -20px;
    padding-left: 20px;
    padding-right: 20px;
    border-bottom: 1px solid rgba(255,255,255,0.1);
  }

  .edit-avatar {
    width: 85px;
    height: 85px;
    border-radius: 50%;
    object-fit: cover;
    margin-bottom: 10px;
  }

  .edit-name {
    font-size: 18px;
    font-weight: 600;

  }

  .edit-sub {
    font-size: 12px;
    opacity: 0.7;
    text-align: center;
  }

  .input {
    width: 100%;
    margin-top: 5px;
    padding: 10px;
    border-radius: 8px;
    border: none;
    background: #0b2a3c;
    color: white;
    outline: none;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  }

  .form-group {
    display: flex;
    flex-direction: column;
    margin-top: 10px;
  }
  .form-group label {
    font-size: 13px;
    margin-bottom: 2px;
    opacity: 0.7;
  }



  .modal-footer {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 80px;
    padding-top: 20px;
    border-top: 1px solid rgba(255,255,255,0.1);
  }

  .save {
    background: #003867; 
    color: white;             
    padding: 8px 15px;
    border-radius: 6px;
    border: none;
    cursor: pointer;
    transition: none;         
  }

  .save:focus {
    outline: none; 
  }

  .close-btn {
    all: unset;
    cursor: pointer;
    font-size: 18px;
  }

  .cancel {
    background: transparent;
    color: white;
    border: none;
    cursor: pointer;
    padding: 0; 
    border-radius: 0;
    outline: none;
  }
</style>