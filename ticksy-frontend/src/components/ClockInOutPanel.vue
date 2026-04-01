<template>
    <transition name="slide-workschedule">
        <div v-if="isOpen" 
            class="clockInOut-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="clockInOut-header">
                <h3>Clock In/Out</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="clockInOut-content">
                <div class="user-profile">
                    <div class="avatar-profile">
                        <img :src="avatar" class="avatar" />
                        <User :size="38" color="#fffff"/>
                    </div>
                    <div class="user-info">
                        <span class="user-name">IDA Admin</span> <!--HARDCODED-->
                        <span class="user-status">Last in 2 hours ago</span> <!--HARDCODED-->
                    </div>
                </div>
            </div>

            <div class="tabs">
                <button 
                    class="tab-btn" 
                    :class="{ active: mode === 'in' }" 
                    @click="mode = 'in'"
                > Clock In
                </button>
                <button 
                    class="tab-btn" 
                    :class="{ active: mode === 'out' }" 
                    @click="mode = 'out'"
                > Clock Out
                </button>
            </div>

            <div class="form-body">
                <div class="input-field">
                    <span>2:40 pm</span>
                    <Clock :size="18"/>
                </div>

                <div class="input-field">
                    <span>Today</span>
                    <Calendar :size="18"/>
                </div>

                <div class="textarea-field">
                    <textarea placeholder="Add a Note" v-model="note"></textarea>
                </div>
            </div>

            <div class="footer-actions">
                <button class="btn-cancel">Cancel</button>
                <button class="btn-save" @click="handleSave">Save</button>
            </div>
        </div>
    </transition>
</template>

<script setup>
    import { ref } from "vue";
    import {X, CheckCircle, User, Clock, Calendar} from "lucide-vue-next";
    import avatar from "../assets/sample_img.jpg";
    import DimmedBg from "./DimmedBg.vue";

    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean
    });

    const emit = defineEmits(['close', 'save']);
    const showToast = ref(false)
    const mode = ref('in'); 

    const handleSave = () => {
        showToast.value = true;
        setTimeout(() => {
            showToast.value = false;
            emit('save'); 
        }, 800);
    };

</script>

<style scoped>

    .clockInOut-panel {
        position: fixed;
        top: 0;
        right: 0;
        bottom: 0;
        width: 420px;
        background-color: #001527;
        z-index: 2000 !important;
        display: flex;
        flex-direction: column;
        color: white;
        box-shadow: -10px 0 30px rgba(0, 0, 0, 0.5);
    }

    .clockInOut-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .clockInOut-header h3 {
        font-size: 1.2rem;
        font-weight: 500;
        margin-left: 30px;
        font-family: Righteous;
        font-size: 20px;
    }

    .clockIn-content {
        flex: 1;
        padding: 20px;
    }

    .user-profile {
        display: flex;
        align-items: center;
        gap: 25px;
        margin: 30px;
    }

    .avatar {
        width: 85px;
        height: 85px;
        border-radius: 50%;
        object-fit: cover;
    }

    .avatar-profile {
        width: 64px;
        height: 64px;
        border-radius: 50%;
        border: 2px solid rgba(255, 255, 255, 0.5);
        display: flex;
        align-items: center;
        justify-content: center;
        margin-left: 25px;
        background: rgba(255, 255, 255, 0.05);
    }

    .user-info {
        display: flex;
        flex-direction: column;
    }

    .user-name {
        font-weight: 600;
        font-size: 16px;
    }

    .user-status {
        font-size: 12px;
        color: rgba(255, 255, 255, 0.6);
    }

    .tabs{
        display: flex;
        gap: 10px;
        padding: 5px;
        border-top: 1px solid rgba(255, 255, 255, 0.2);
        border-bottom: 1px solid rgba(255, 255, 255, 0.2);
    }

    .tabs button{
        flex: 1;
        background: none;
        border: none;
        outline: none;
        color: rgba(255, 255, 255, 0.6);
        padding: 12px;
        cursor: pointer;
        font-weight: 500;
        position: relative;
        font-size: 14px;
    }

    .tabs button::after {
        content: "";
        position: absolute;
        bottom: -5px;
        left: 31%;
        transform: translateX(-50%);
        width: 0;
        height: 3.1px;
        border-radius: 3px;
        background: rgb(117, 136, 154, 50%);
        transition: width 0.3s ease-in-out;
    }

    .tabs button.active {
        color: #88b6ff;
        font-weight: 600;
        transition: color 0.3s ease;
    }

    .tabs button.active::after {
        width: 60%;
        margin-left: 40px;
        background: #88b6ff; 
        box-shadow: 0 0 8px rgba(59, 130, 246, 0.5);
    }

    .tabs button:hover:not(.active) {
        color: rgba(255, 255, 255, 0.8);
        cursor: pointer;
    }

    .icon-btn {
        background: transparent;
        border: none;
        color: white;
        cursor: pointer;
        opacity: 0.7;
    }

    .schedule-content {
        flex: 1;
        overflow-y: auto;
        padding: 24px;
    }

    .schedule-content::-webkit-scrollbar {
        width: 8px;
    }

    .schedule-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .schedule-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }

    .schedule-content::-webkit-scrollbar-track {
        background: transparent; 
    }

    .form-body {
        padding: 32px 24px;
        display: flex;
        flex-direction: column;
        gap: 20px;
    }

    .input-field {
        background: #00233d;
        font-size: 14px;
        border-radius: 8px;
        padding: 16px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 12px;
        color: rgba(255, 255, 255, 0.9);
    }

    .textarea-field textarea {
        width: 100%;
        font-size: 14px;
        height: 140px;
        background: #00233d;
        border: none;
        border-radius: 6px;
        padding: 16px;
        color: white;
        resize: none;
        outline: none;
        font-family: inter;
    }

    .textarea-field textarea::-webkit-scrollbar {
        width: 8px;
    }

    .textarea-field textarea::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .textarea-field textarea::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }

    .textarea-field textarea::-webkit-scrollbar-track {
        background: transparent; 
    }

    .footer-actions {
        margin-top: auto;
        padding: 24px;
        display: flex;
        gap: 12px;
        border-top: 1px solid rgba(255, 255, 255, 0.1);
        justify-content: flex-end;
    }

    .btn-cancel, .btn-save {
        padding: 10px 20px;
        min-width: 120px;
        height: fit-content;
        border-radius: 6px;
        border: none;
        cursor: pointer;
        font-weight: 500;
    }

    .btn-cancel {
        background: #003867;
        color: white;
        outline: none;
    }

    .btn-cancel:hover {
        opacity: 0.8;
    }

    .btn-save {
        background: rgb(0, 56, 103, 50%);
        color: white;
        outline: none;
    }

    .btn-save:hover {
        opacity: 0.8;
    }

    .slide-workschedule-enter-active,
    .slide-workschedule-leave-active {
        transition: transform 0.3s ease;
    }

    .slide-workschedule-enter-from,
    .slide-workschedule-leave-to {
        transform: translateX(100%);
    }

    .success-toast {
        position: absolute;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        background: #06d6a0;
        color: #001324;
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        box-shadow: 0 4px 15px rgba(6, 214, 160, 0.3);
        z-index: 2000;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }
</style>