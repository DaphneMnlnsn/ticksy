<template>
    <div class="panel">
        <div class="panel-header">
            <slot name="header"></slot>
        </div>

        <div class="panel-body">
            <slot name="body"></slot>
        </div>

        <div class="panel-footer">
            <button @click="$emit('actionClick')" class="btn">
                {{ buttonText }}
            </button>
        </div>

    </div>
</template>

<script setup>
    defineProps({
        buttonText: {
            type: String,
            default: "Submit"
        }
    })
</script>

<style scoped>
    .panel {
        width: 260px;
        display: flex;
        flex: 1;
        flex-direction: column;
        background: #001527;
    }

    .panel-header {
        padding: 10px;
    }

    .panel-body {
        flex: 1;
        overflow-y: auto;
    }

    .panel-footer {
        margin-top: 10px;
    }

    .btn {
        background-color: #052348;
        width: 100%;
        border-radius: 0;
        font-size: 14px;
        padding: 10px;
        outline: none;
        border: none;
    }
</style>