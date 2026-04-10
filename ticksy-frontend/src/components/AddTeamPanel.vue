<template>
  <div v-if="modelValue" class="overlay">
    
    <div class="side-panel">

      <div class="panel-header">
        <span>Create Team</span>
        <span class="close" @click="close">✕</span>
      </div>

      <div class="panel-body">

        <div class="form-group">
          <label>Team Name</label>
          <input v-model="teamName" placeholder="DWM Team" />
        </div>

        <div class="form-group">
          <label>Group Managers</label>
          <div class="user-box">
            <img :src="avatar" class="avatar" />
            <UserPlus class="add-icon" />
          </div>
        </div>

        <div class="form-group">
          <label>Members</label>
          <div class="user-box multi">
            <img :src="avatar" class="avatar" />
            <img :src="avatar" class="avatar" />
            <img :src="avatar" class="avatar" />
            <UserPlus class="add-icon" />
          </div>
        </div>

      </div>

      <div class="panel-footer">
        <button class="cancel" @click="close">Cancel</button>
        <button class="save" @click="save">Save</button>
      </div>

    </div>

  </div>
</template>

<script setup>
  import { ref } from 'vue'
  import { UserPlus } from 'lucide-vue-next'

  const props = defineProps({
    modelValue: Boolean,
    avatar: String
  })

  const emit = defineEmits(['update:modelValue', 'save'])

  const teamName = ref('')

  function close() {
    emit('update:modelValue', false)
  }

  function save() {
    emit('save', {
      name: teamName.value,
      members: []
    })
    close()
  }

</script>

<style scoped>

  .overlay {
    position: fixed;
    inset: 0;
    background: rgba(0,0,0,0.6);
    z-index: 999;
  }

  .side-panel {
    position: absolute;
    right: 0;
    top: 0;
    width: 380px;
    height: 100%;
    background: #031e2f;
    display: flex;
    flex-direction: column;
    animation: slideIn 0.3s ease;
    color: white;
  }

  .panel-header {
    display: flex;
    justify-content: space-between;
    padding: 18px;
    font-weight: 600;
    border-bottom: 1px solid rgba(255,255,255,0.1);
  }

  .close {
    cursor: pointer;
  }

  .panel-body {
    padding: 20px;
    flex: 1;
  }

  .form-group {
    margin-bottom: 20px;
  }

  .form-group label {
    font-size: 12px;
    opacity: 0.7;
  }

  input {
    width: 100%;
    padding: 10px;
    margin-top: 6px;
    border-radius: 8px;
    border: none;
    background:  #0b2a3c;
    color: white;
  }

  .user-box {
    margin-top: 8px;
    background:  #0b2a3c;
    padding: 10px;
    border-radius: 10px;
    display: flex;
    align-items: center;
    gap: 8px;
  }

  .add-icon {
    margin-left: auto;
  }

  .user-box.multi .avatar {
    margin-left: -8px;
    border: 2px solid #031e2f;
  }

  .avatar {
    width: 32px;
    height: 32px;
    border-radius: 50%;
  }

  .add-icon {
    width: 18px;
    height: 18px;
    stroke: white;
    opacity: 0.8;
    cursor: pointer;

    flex-shrink: 0;
  }

  .add-icon svg {
    width: 18px;
    height: 18px;
  }

  .add-icon:hover {
    opacity: 1;
    transform: scale(1.1);
  }

  .panel-footer {
    display: flex;
    justify-content: space-between;
    padding: 15px 20px;
    border-top: 1px solid rgba(255,255,255,0.1);
  }

  .cancel {
    background: none;
    border: none;
    color: white;
  }

  .save {
    background: #003867;
    border: none;
    padding: 8px 16px;
    border-radius: 6px;
    color: white;
  }

  @keyframes slideIn {
    from { transform: translateX(100%); }
    to { transform: translateX(0); }
  }


</style>