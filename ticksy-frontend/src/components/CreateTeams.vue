<template>
  <div v-if="isOpen" class="overlay" @click.self="close">
    <div class="panel">
      
      <!-- Header -->
      <div class="panel-header">
        <h2>Create Team</h2>
        <button class="close-btn" @click="close">✕</button>
      </div>

      <!-- Body -->
      <div class="panel-body">

        <!-- Team Name -->
        <div class="form-group">
          <label>Team Name</label>
          <input v-model="teamName" type="text" placeholder="Enter team name" />
        </div>

        <!-- Group Managers -->
        <div class="form-group">
          <label>Group Managers</label>
          <div class="avatar-box">
            <div class="avatars">
              <img v-for="(m, i) in managers" :key="i" :src="m" />
            </div>
            <button class="add-btn">+</button>
          </div>
        </div>

        <!-- Members -->
        <div class="form-group">
          <label>Members</label>
          <div class="avatar-box">
            <div class="avatars">
              <img v-for="(m, i) in members" :key="i" :src="m" />
            </div>
            <button class="add-btn">+</button>
          </div>
        </div>

      </div>

      <!-- Footer -->
      <div class="panel-footer">
        <button class="cancel" @click="close">Cancel</button>
        <button class="save" @click="save">Save</button>
      </div>

    </div>
  </div>
</template>

<script>
export default {
  props: {
    isOpen: Boolean
  },
  data() {
    return {
      teamName: "DWM Team",
      managers: [
        "https://i.pravatar.cc/40?img=1"
      ],
      members: [
        "https://i.pravatar.cc/40?img=2",
        "https://i.pravatar.cc/40?img=3",
        "https://i.pravatar.cc/40?img=4"
      ]
    }
  },
  methods: {
    close() {
      this.$emit("close")
    },
    save() {
      this.$emit("save", {
        name: this.teamName,
        managers: this.managers,
        members: this.members
      })
    }
  }
}
</script>

<style scoped>
/* Overlay */
.overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.4);
  display: flex;
  justify-content: flex-end;
  z-index: 1000;
}

/* Panel */
.panel {
  width: 350px;
  height: 100%;
  background: #0b2a44;
  color: white;
  display: flex;
  flex-direction: column;
  box-shadow: -5px 0 20px rgba(0,0,0,0.4);
}

/* Header */
.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid rgba(255,255,255,0.1);
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 18px;
  cursor: pointer;
}

/* Body */
.panel-body {
  flex: 1;
  padding: 20px;
}

/* Form */
.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  font-size: 13px;
  margin-bottom: 8px;
  opacity: 0.8;
}

input {
  width: 100%;
  padding: 10px;
  border-radius: 6px;
  border: none;
  background: #123a5a;
  color: white;
}

/* Avatar box */
.avatar-box {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #123a5a;
  padding: 10px;
  border-radius: 8px;
}

.avatars {
  display: flex;
}

.avatars img {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  margin-right: -8px;
  border: 2px solid #0b2a44;
}

/* Add button */
.add-btn {
  background: #1e5b8c;
  border: none;
  color: white;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  cursor: pointer;
}

/* Footer */
.panel-footer {
  display: flex;
  justify-content: flex-end;
  padding: 15px;
  gap: 10px;
  border-top: 1px solid rgba(255,255,255,0.1);
}

.cancel {
  background: transparent;
  border: none;
  color: white;
  cursor: pointer;
}

.save {
  background: #1e5b8c;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  color: white;
  cursor: pointer;
}
</style>