<template>
    <div class="parts">
        <img class="partImage" v-bind:src="part.part_Img_Url" />
        <span class="quantity">{{ part.part_Quantity }}</span>
        <span class="partName">{{ part.part_Name }}</span>
        <span class="colour">{{ part.colour }}</span>
        <span class="rgb" v-bind:style="{ 'background-color': '#' + part.rgb}"></span>
        <p></p>
    </div>
</template>

<script>

export default {
    props: {
        part: {
            type: Object,
            required: true
        }
    } 
}

</script>

<style>
.parts {
    padding: 10px;
    display: grid;
    grid-template-columns: 100px 10% 1fr 15% 5%;
    grid-template-areas:
        "partImage quantity partName colour rgb";
    gap: 5px;
    align-items: center;
}

.partImage {
    grid-area: partImage;
    padding: 5px;
    max-height: 50px;
    max-width: 50px;
    margin-left: auto;
    margin-right: auto;
    display: inline;
}

.quantity {
    grid-area: quantity;
    padding: 5px;
    text-align: center;
}

.partName {
    grid-area: partName;
    padding: 5px;
}

.colour {
    grid-area: colour;
    padding: 5px;
    text-align: center; 
}

.rgb {
    grid-area: rgb;
    padding: 5px;
    height: 40px;
    text-align: center;
}
</style>