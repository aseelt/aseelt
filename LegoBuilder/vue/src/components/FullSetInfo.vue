<template>
    <div class="set">
        <img class="image" v-bind:src="fullSetInfo.set_Img_Url" />
        <div class="info">
            <h1>{{ fullSetInfo.set_Num }} | {{ fullSetInfo.set_Name }}</h1>
            <h2>{{ fullSetInfo.total_Num_Parts }}</h2>
        </div>

        <div class="tableHead">
            <span class="partImageHead">Example</span>
            <span class="quantityHead">Quantity</span>
            <span class="partNameHead">Part Name</span>
            <span class="partColourHead">Colour</span>
            <p></p>
        </div>

        <table class="part">
            <tr>
                <parts v-for="part in fullSetInfo.parts" v-bind:key="part.elementId" v-bind:part="part" />
            </tr>
        </table>
    </div>
</template>

<script>
import Parts from '../components/Parts.vue'

export default {
    components: {
        Parts
    },
    props: {
        fullSetInfo: {
            type: Object,
            required: true
        }
    }
}

</script>

<style>
.set {
    padding: 20px;
    display: grid;
    grid-template-columns: 300px 1fr;
    grid-template-areas:
        "image info" 
        "tableHeader tableHeader"
        "parts parts";
    gap: 10px
}

.image {
    grid-area: image;
    display: inline;
}

.info {
    grid-area: info;
}
 
.tableHead {
    padding: 10px;
    display: grid;
    grid-area: tableHeader;
    grid-template-columns: 100px 10% 1fr 15% 5%;
    grid-template-areas:
        "partImageHeader quantityHeader partNameHeader partColourHeader partColourHeader";
    gap: 5px;
    align-items: center;
}

.partImageHead {
    grid-area: partImageHeader;
    text-align: center;
    padding: 5px;
}

.quantityHead {
    grid-area: quantityHeader;
    text-align: center;
    padding: 5px;
}

.partNameHead {
    grid-area: partNameHeader;
    padding: 5px;
}

.partColourHead {
    grid-area: partColourHeader;
    text-align: center;
    padding: 5px;
}

.part {
    grid-area: parts;
}
</style>