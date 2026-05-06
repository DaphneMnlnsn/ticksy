<template>
    <DimmedBg :show="isOpen" @close="emit ('close')"></DimmedBg>

    <transition name="slide-workschedule">
        <div v-if="isOpen" 
            class="policy-panel"
            :style="{ borderright: isSidebarCollapsed ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)' }"
        >   
            <transition name="fade">
                <div v-if="showToast" class="success-toast">
                    <CheckCircle :size="18" />
                    <span>Saved successfully!</span>
                </div>
            </transition>

            <div class="policy-header">
                <h3>Time Off Policies</h3>
                <button class="icon-btn" @click="emit('close')"><X/></button>
            </div>
            
            <div class="policy-content">
                <transition name="fade-slide" mode="out-in">
                    <div
                        v-if="!isAddPolicyOpen && !isEditPolicyOpen"
                        key="list"
                        class="list-wrapper"
                    >
                    <div class="search-row">
                        <div class="search-box">
                        <Search v-model="searchQuery" placeholder="Search policies..." />
                        </div>

                        <button class="create-btn" @click="handleAddPolicy">
                        <Plus class="create-icon"/>
                        <span>Create Policy</span>
                        </button>
                    </div>

                    <div class="holiday-table-wrapper">
                        <table class="timeoff-table">
                            <thead>
                                <tr>
                                <th><div class="table-header">Name</div></th>
                                <th><div class="table-header">Max Days</div></th>
                                <th><div class="table-header">Rules/Description</div></th>
                                <th><div class="table-header">Actions</div></th>
                                </tr>
                            </thead>

                            <tbody>
                                <tr v-for="policy in filteredPolicies" :key="policy.id">
                                <td style="font-weight: 500;">{{ policy.name }}</td>
                                <td>{{ policy.details ? policy.details.maxDays : '...' }}</td>
                                <td style="max-width: 200px; word-break: break-word; white-space: normal;">
                                    {{ policy.details ? policy.details.rules : 'Loading...' }}
                                </td>
                                <td>
                                    <div class="actions">
                                    <span class="dots" @click="toggleMenu(policy.id)">•••</span>
                                    <div v-if="activeMenu === policy.id" class="dropdown">
                                        <div class="dropdown-item" @click="handleEditPolicy(policy)">
                                        <Edit2 size="14" /> Edit
                                        </div>
                                        <div class="dropdown-item reject" @click="handleDeletePolicy(policy.id)">
                                        <Trash2 size="14" /> Delete
                                        </div>
                                    </div>
                                    </div>
                                </td>
                                </tr>
                            </tbody>
                            </table>
                        </div>
                    </div>

                    <div
                        v-else
                        key="form"
                        class="form-container-inner"
                    >
                    <div class="form-group">
                        <div class="input-group-policyName">
                        <div class="label-header">
                            <label class="form-label">Policy Name</label>
                            <span v-if="errors.name" class="error-msg">*Required</span>
                        </div>
                        <input type="text" v-model="form.name" placeholder="e.g. Annual Leave"/>
                        </div>

                        <div class="input-group-maxDays">
                        <div class="label-header">
                            <label class="form-label">Max Days</label>
                            <span v-if="errors.maxDays" class="error-msg">*Enter a number</span>
                        </div>
                        <div class="duration-input large">
                            <input type="number" v-model="form.maxDays" min="0" class="time-box" />
                        </div>
                        </div>
                    </div>

                    <div class="input-group">
                        <div class="label-header">
                            <label class="form-label">Rules / Description</label>
                            </div>
                            <textarea v-model="form.rules" class="form-textarea"
                            placeholder="Describe eligibility, carry-over rules, etc...">
                            </textarea>
                        </div>
                    </div>
                </transition>
            </div>

            <div class="footer-actions">
                        <button class="btn-cancel" @click="closeForms">Cancel</button>
                        <button class="btn-save" @click="handleSavePolicy">
                            {{ isEditPolicyOpen ? 'Update Policy' : 'Save Policy' }}
                        </button>
            </div>

        </div>
    </transition>
</template>

<script setup>
    import { ref, computed, onMounted } from "vue";
    import { X, CheckCircle, Plus, Edit2, Trash2 } from "lucide-vue-next";
    import { getPolicies, getPolicyDetails, deletePolicy, createPolicy, updatePolicy } from "../services/timeOff";
    import Search from "./Search.vue";
    import DimmedBg from "./DimmedBg.vue";
    import Swal from "sweetalert2"; 

    const toastMessage = ref('');

    const props = defineProps({
        isOpen: Boolean,
        isSidebarCollapsed: Boolean
    });
    const emit = defineEmits(['close']);

    const searchQuery = ref("");
    const policies = ref([]);
    const showToast = ref(false);
    const activeMenu = ref(null);
    const isAddPolicyOpen = ref(false);
    const isEditPolicyOpen = ref(false);
    const selectedPolicy = ref(null);
    const isLoading = ref(false);

    const form = ref({
        name: '',
        maxDays: 0,
        rules: ''
    });

    const errors = ref({ name: false, maxDays: false });

    const loadPolicies = async () => {
        try {
            isLoading.value = true;
            const list = await getPolicies();
            
            const detailedPolicies = await Promise.all(
                list.map(async (p) => {
                    const detail = await getPolicyDetails(p.id);
                    return { ...p, details: detail };
                })
            );
            
            policies.value = detailedPolicies;
        } catch (err) {
            console.error("Failed to load policies:", err);
        } finally {
            isLoading.value = false;
        }
    };

    const handleSavePolicy = async () => {
        errors.value.name = !form.value.name.trim();
        errors.value.maxDays = form.value.maxDays === null || form.value.maxDays < 0;
        if (errors.value.name || errors.value.maxDays) return;

        const payload = {
            name: form.value.name,
            maxDays: form.value.maxDays,
            rules: form.value.rules
        };

        try {
            if (isEditPolicyOpen.value && selectedPolicy.value) {
                await updatePolicy(selectedPolicy.value.id, payload);
            } else {
                await createPolicy(payload);
            }

            triggerToast();
            await loadPolicies();
            closeForms();
        } catch (err) {
            console.error("Save failed:", err);
            alert("Failed to save policy. Please try again.");
        }
    };

    const handleDeletePolicy = async (id) => {
        const result = await Swal.fire({
            title: 'Delete Policy',
            text: 'Are you sure you want to delete this policy? This action cannot be undone.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel',
            confirmButtonColor: "#d33",
            cancelButtonColor: "#6c757d",
        });

        if (result.isConfirmed) {
            try {
                await deletePolicy(id);
                policies.value = policies.value.filter(p => p.id !== id);
                activeMenu.value = null;

                Swal.fire({
                    title: 'Deleted!',
                    text: 'Policy has been deleted.',
                    icon: 'success',
                    timer: 1500,
                    showConfirmButton: false
                });

                triggerToast("Policy deleted successfully!");
            } catch (err) {
                console.error("Delete failed:", err.response || err);

                Swal.fire({
                    title: 'Error',
                    text: 'Could not delete policy.',
                    icon: 'error',
                });
            }
        }
    };

    const triggerToast = (message) => {
        toastMessage.value = message;
        showToast.value = true;

        setTimeout(() => {
            showToast.value = false;
        }, 2000);
    };

    const handleAddPolicy = () => {
        resetForm();
        isAddPolicyOpen.value = true;
    };

    const handleEditPolicy = (policy) => {
        form.value = {
            name: policy.name,
            maxDays: policy.details?.maxDays || 0,
            rules: policy.details?.rules || ''
        };
        selectedPolicy.value = policy;
        isEditPolicyOpen.value = true;
        activeMenu.value = null;
    };

    const resetForm = () => {
        form.value = { name: '', maxDays: 0, rules: '' };
        errors.value = { name: false, maxDays: false };
    };

    const closeForms = () => {
        isAddPolicyOpen.value = false;
        isEditPolicyOpen.value = false;
        selectedPolicy.value = null;
        resetForm();
    };

    const toggleMenu = (id) => {
        activeMenu.value = activeMenu.value === id ? null : id;
    };

    const filteredPolicies = computed(() => {
        const query = (searchQuery.value || "").toLowerCase().trim();
        if (!query) return policies.value;

        return policies.value.filter(p => {
            const nameMatch = p.name?.toLowerCase().includes(query);
            const maxDaysStr = p.details?.maxDays != null ? String(p.details.maxDays) : "";
            return nameMatch || maxDaysStr.includes(query);
        });
    });

    onMounted(() => {
        loadPolicies();
    });
</script>

<style scoped>

    .policy-panel {
        position: fixed;
        top: 0;
        right: 0;
        bottom: 0;
        width: 35%;
        background-color: #001527;
        z-index: 1000;
        display: flex;
        flex-direction: column;
        color: white;
        box-shadow: -10px 0 30px rgba(0, 0, 0, 0.5);
    }

    .policy-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 5px 0 0;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    }

    .policy-header h3 {
        font-size: 1.2rem;
        font-weight: 500;
        margin-left: 30px;
        margin-top: 18px;
        font-family: Righteous;
        font-size: 20px;
    }

    .label-header{
        display: flex;
        width: 100%;
        align-items: center;
        padding-right: 12px;
    }

    .icon-btn {
        background: transparent;
        border: none;
        color: white;
        cursor: pointer;
        opacity: 0.7;
        outline: none;
    }

    .policy-content {
        flex: 1;
        overflow-y: auto;
        padding: 30px ;
        display: block;
        width: 100%;
    }

    .policy-content::-webkit-scrollbar {
        width: 8px;
    }

    .policy-content::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.1); 
        border-radius: 10px; 
        border: 2px solid transparent; 
        background-clip: content-box; 
    }

    .policy-content::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.25); 
    }


    .policy-content::-webkit-scrollbar-track {
        background: transparent; 
    }
    
    .input-group {
        margin-bottom: 24px;
    }

    .form-label {
        display: block;
        font-size: 0.85rem;
        color: #ffffff;
        margin-bottom: 2%;
    }

    .form-group {
        display: flex;
        gap: 85px;
        align-items: flex-start;
        margin-bottom: 15px;
    }

    .policyName {
        flex: 1;
        min-width: 0;
        padding: 10px 15px;
    }

    .maxDays {
        width: 120px;
    }

    input[type="text"] {
        width: 100%;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        padding: 10px 12px;
        border-radius: 6px;
        color: white;
        outline: none;
    }

    input[type="number"] {
        width: 100%;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        padding: 10px 12px;
        border-radius: 6px;
        color: white;
        outline: none;
    }

    .error-msg {
        color: rgb(255, 99, 99);
        font-size: 0.65rem;
        font-weight: 500;
        letter-spacing: 0.3px;
        animation: errorShake 0.4s ease-in-out;
        margin-left: auto !important;
        text-align: right;
        white-space: nowrap;
    }

    @keyframes errorShake {
        0%, 100% { transform: translateX(0); }
        25% { transform: translateX(4px); }
        75% { transform: translateX(-4px); }
    }

    .slide-workschedule-enter-active,
    .slide-workschedule-leave-active {
        transition: transform 0.3s ease;
    }

    .slide-workschedule-enter-from,
    .slide-workschedule-leave-to {
        transform: translateX(100%);
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity 0.3s ease, transform 0.3s ease;
    }
    .fade-enter-from, .fade-leave-to {
        opacity: 0;
        transform: translate(-50%, -10px);
    }

    .fade-slide-enter-active,
    .fade-slide-leave-active {
        transition: all 0.3s ease;
    }

    .fade-slide-enter-from {
        opacity: 0;
        transform: translateX(20px);
    }

    .fade-slide-leave-to {
        opacity: 0;
        transform: translateX(-20px);
    }

    .holiday-table-wrapper table {
        width: 100%;
        border-collapse: collapse;
    }

    .table-header {
        display: flex;
        align-items: center;
        gap: 10px;
        width: 100%;
        justify-content: flex-start;
        padding: 10px 20px;
    }

    th {
        text-align: left;
        white-space: nowrap;
        font-size: 13px;
        font-weight: 400;
    }

    thead {
        background-color: #75889A4D;
    }
    
    td {
        padding: 10px 20px;
        font-size: 12px;
    }

    .title-group, .card-title2 {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
    }

    .btn-group {
        display: flex;
        flex-direction: row;
        gap: 10px;
    }

    .icon, .create-icon {
        width: 20px;
        height: 20px;
        cursor: pointer;
    }

    .icon:hover, .create-btn:hover, .add-btn:hover, .assign-btn:hover {
        opacity: 0.8;
    }

    .search-row {
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 100%;
        margin-bottom: 15px;
    }

    .search-box {
        width: 50%;
    }
    
    .create-btn {
        display: flex;
        justify-content: center;
        align-items: center;
        font-size: 14px;
        gap: 10px;
        background: none;
        outline: none;
        border: none;
        cursor: pointer;
    }

    .edit-actions {
        display: inline-flex;
        align-items: center;
        gap: 1px;
        margin-left: 10px;
        vertical-align: middle;
    }

    .delete-text {
        color: #ef4444;
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
        z-index: 9999;
    }

    .delete-toast {
        position: absolute;
        top: 55px;
        left: 50%;
        transform: translateX(-50%);
        background: #d60606;
        color: #d8dee3;
        padding: 10px 20px;
        border-radius: 30px;
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
        font-weight: 600;
        box-shadow: 0 4px 15px rgba(214, 6, 6, 0.3);
        z-index: 9999;
    }

    .actions {
        position: relative;
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .dots {
        cursor: pointer;
        font-size: 10px;
        opacity: 0.7;
    }

    .dots:hover {
        opacity: 1;
    }

    .dropdown {
        position: absolute;
        right: 0;
        top: 22px;
        width: 140px;
        background: #061a2b; 
        border-radius: 12px;
        padding: 6px;
        z-index: 100;
        overflow: hidden;
        box-shadow: 0 10px 25px rgba(0,0,0,0.3);
    }

    .dropdown-item {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 10px;
        cursor: pointer;
        font-size: 12px;
    }

    .dropdown-item:hover {
        background: rgba(255,255,255,0.08);
    }

    .reject {
        color: #ff4d4d;
    }

    @keyframes fadeIn {
        from { opacity: 0; transform: translateY(10px); }
        to { opacity: 1; transform: translateY(0); }
    }

    .form-overlay {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: #001527;
        z-index: 1100;
        padding: 20px 40px;
    }

    .form-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 20px;
    }

    .slide-up-enter-active, .slide-up-leave-active {
        transition: all 0.3s ease-out;
    }
    .slide-up-enter-from, .slide-up-leave-to {
        transform: translateY(100%);
    }

    .form-container-inner {
        display: block;
        height: 100%;
    }

    .input-group {
        display: flex;
        flex-direction: column;
        width: 100%;
    }

    .form-textarea {
        width: 100%;
        min-height: 120px;
        max-height: 100%;
        display: block;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        padding: 12px;
        border-radius: 8px;
        color: white;
        outline: none;
        resize: vertical;
        font-family: inherit;
    }

    .form-textarea:focus {
        border-color: #3b82f6;
        background: rgba(255, 255, 255, 0.08);
    }

    .time-box {
        width: 80px;
        height: 40px;
        background: #001e36;
        border: 1px solid rgba(255, 255, 255, 0.1);
        border-radius: 8px;
        color: white;
        text-align: center;
        font-weight: 600;
    }

    .footer-actions {
        padding: 16px 24px;
        display: flex;
        gap: 12px;
        border-top: 1px solid rgba(255, 255, 255, 0.1);
        justify-content: flex-end;
        background: #001527; 
        margin: 10px 0;   
        z-index: 10;
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
</style>