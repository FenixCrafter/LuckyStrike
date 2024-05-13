<template>
  <loader v-if="loading" />
  <div class="productBeheer-container">
    <div class="title-container">
      <h1 class="title">Beheer producten</h1>
      <div class="voegProductToeBtn" @click="openModal(false, productModel)">Voeg product toe</div>
      <div class="voegProductToeBtn" @click="categoryModal = true">Bewerk categoriëen</div>
    </div>
    <div class="content-container">
      <div v-for="(product, category) in products" class="category" :class="`category${category.replace(' ', '_')}`">
        <h2 @click="handleCollapse(`category${category.replace(' ', '_')}`)">{{ category }} <img class="categoryArrow" src="@/assets/img/icon/arrow-right.svg" alt=""></h2>
        <div class="items collapsed">
          <product-card v-for="singleProduct in product" :product="singleProduct" @click="openModal(true, singleProduct)" />
        </div>
      </div>
    </div>
  </div>
  <div class="editModalOuterContainer" v-if="modal">
    <modal @close="closeModal" class="modal" :title="modalTitle"  width="500px">
      <div class="editModalContainer">
        <label for="category">Categorie</label>
        <select id="category" name="category" v-model="productModel.categoryId" size="4">
          <option :value="category.id" v-for="category in categories">{{ category.name }}</option>
        </select>
        <label for="name">Naam</label>
        <input id="name" v-model="productModel.name" type="text" placeholder="Naam" name="naam">
        <label for="price">Prijs</label>
        <input id="price" name="price" @keyup="onPriceChange" v-model="productModel.price" type="text" placeholder="Prijs">
        <label for="img">Afbeelding</label>
        <input ref="fileInput" type="file" @change="fileChanged" name="img " id="img" placeholder="Foto">
        <div class="buttonContainer">
          <div class="createProductBtn productBtn fullWidth" @click="createProduct()" v-if="!editMode">Maak product aan</div>
          <div class="createProductBtn productBtn" v-if="editMode" @click="editProduct()">Pas product aan</div>
          <div class="deleteProductBtn productBtn" v-if="editMode" @click="deleteProduct()">Verwijder product</div>
        </div>
      </div>
    </modal>
  </div>

  <div class="editModalOuterContainer" v-if="categoryModal">
    <modal @close="categoryModal = false" class="modal" title="Bewerk categoriëen"  width="500px">
      <div class="editModalContainer">
        <label for="category">Categorie</label>
        <select id="category" name="category" v-model="editCategoryId" @change="selectedCategoryChanged" size="4">
          <option :value="category.id" v-for="category in categories">{{ category.name }}</option>
        </select>
        <label for="name">Naam</label>
        <input id="name" v-model="editCategoryName" type="text" placeholder="Naam" name="naam">
        <div class="buttonContainer">
          <div class="createProductBtn productBtn fullWidth" @click="newCategory()">Nieuw</div>
          <div class="createProductBtn productBtn fullWidth" @click="editCategory()">Bewerk</div>
          <div class="deleteProductBtn productBtn" @click="deleteCategory()">Verwijder</div>
        </div>
      </div>
    </modal>
  </div>
</template>

<script lang="ts">
import {defineComponent} from "vue";
import ProductCard from "@/components/productCard.vue";
import modal from "@/components/modal.vue";
import { CategoryClient, Category, Product, ProductClient } from '!/ApiClient';
import type { FileParameter } from '!/ApiClient';
import axios from "axios";
import loader from "@/components/loader.vue";

export default defineComponent({
  components: {ProductCard, modal, loader},
  data() {
    return{
      loading: false,
      products: {} as { [category: string]: Product[] },
      categories: [] as Category[],
      modal: false,
      editMode: false,
      productModel: {} as Product,
      priceInput: '',
      modalTitle: '',
      file: null as any,
      axios: axios,
      categoryModal: false,
      editCategoryName: '',
      editCategoryId: null
    }
  },
  methods:{
    handleCollapse(reference:string){
      const element = document.querySelector(`.${reference}`)
      const arrowElement = element?.querySelector(".categoryArrow")
      const childElement = element?.querySelector(".items")
      arrowElement?.classList.toggle("turnedArrow")
      childElement?.classList.toggle("collapsed")
    },
    openModal(mode:boolean, model:Product){
      this.editMode = mode;
      this.setModalTitle(mode);
      this.file = null;
      if (mode){
        this.setProductModel(model)
        fetch(`${axios.defaults.baseURL}/FileStorage/${model.name}.png`)
        .then(response => response.blob())
        .then(blob => {
          let file = new File([blob], `${model.name}.png`);
          let data = new DataTransfer();
          data.items.add(file);
          this.$refs.fileInput.files = data.files;
        })
      }
      this.modal = true
    },
    closeModal(){
      this.modal = false
      this.productModel = {} as Product
    },
    setModalTitle(mode:boolean){
      if (mode){
        this.modalTitle = "Wijzig product"
      } else {
        this.modalTitle = "Maak product aan"
      }
    },
    setProductModel(model:Product){
      this.productModel = model;
      this.onPriceChange();
    },
    createProduct() {
      if (this.file == null)
        return this.$toast.warning("Upload een afbeelding!");

      this.loading = true;
      //@ts-ignore
      new ProductClient(null, this.http).createProduct(this.productModel.name, this.productModel.price.toString().replace(".", ","), this.productModel.categoryId, [{ data: this.file, fileName: this.file.name} as FileParameter])
      .then(() => {
        this.$toast.success("Product aangemaakt.");
        this.closeModal();
        this.fetchProducts();
      }).
          catch((err: ApiException) => {
        this.$toast.error(err.response);
      })
      .finally(() => this.loading = false);
    },
    deleteProduct() {
      this.loading = true;
      //@ts-ignore
      new ProductClient(null, this.http).deleteProduct(this.productModel.id)
      .then(() => {
        this.$toast.success("Product is verwijderd.");
        this.closeModal();
        this.fetchProducts();
      })
      .catch((err: ApiException) => {
        this.$toast.error(err.response);
      })
      .finally(() => this.loading = false);
    },
    fetchProducts() {
      this.loading = true;
      //@ts-ignore
      new ProductClient(null, this.http).getProducts()
      .then((products) => {
        this.products = products;
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    },
    fetchCategories() {
      this.loading = true;
      //@ts-ignore
      new CategoryClient(null, this.http).getAll().then((categories: Category[]) => {
        this.categories = categories;
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    },
    fileChanged(event: any) {
      this.file = event.target.files[0];
    },
    editProduct() {
      let files: FileParameter[] | null;
      if (this.file) {
        files = [{ data: this.file, fileName: this.file.name} as FileParameter];
      }
      this.loading = true;
      //@ts-ignore
      new ProductClient(null, this.http).updateProduct(this.productModel.id, this.productModel.name, this.productModel.price, this.productModel.categoryId, files)
      .then(() => {
        this.$toast.success("Product is bijgewerkt.");
        this.closeModal();
        this.fetchProducts();
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
            console.log(err);
          })
      .finally(() => {
        this.loading = false
        this.closeModal()
      });
    },
    onPriceChange() {
      //@ts-ignore
      this.productModel.price = this.productModel.price.toString().replace(",", ".");
    },
    selectedCategoryChanged() {
      for (let category of this.categories) {
        if (category.id == this.editCategoryId) {
          this.editCategoryName = category.name;
          break;
        }
      }
    },
    newCategory() {
      this.editCategoryId = null;
      this.loading = true;
      //@ts-ignore
      new CategoryClient(null, this.http).add(this.editCategoryName)
      .then(() => {
        this.$toast.success("Categorie aangemaakt!");
        this.editCategoryName = "";
        this.fetchCategories();
        this.fetchProducts();
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    },
    editCategory() {
      if (this.editCategoryId == null)
        return this.$toast.warning("Selecteer eerst een categorie.");

      if (this.editCategoryName == "")
        return this.$toast.warning("Vul een naam in.");
      this.loading = true;
      //@ts-ignore
      new CategoryClient(null, this.http).update(this.editCategoryId, this.editCategoryName)
      .then(() => {
        this.$toast.success("Categorie bewerkt!");
        this.editCategoryName = "";
        this.editCategoryId = null;
        this.fetchCategories();
        this.fetchProducts();
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    },
    deleteCategory() {
      if (this.editCategoryId == null)
        return this.$toast.warning("Selecteer eerst een categorie.");

      this.loading = true;
      //@ts-ignore
      new CategoryClient(null, this.http).delete(this.editCategoryId)
      .then(() => {
        this.$toast.success("Categorie verwijderd!");
        this.editCategoryName = "";
        this.editCategoryId = null;
        this.fetchCategories();
        this.fetchProducts();
      })
          .catch((err: ApiException) => {
            this.$toast.error(err.response);
          })
      .finally(() => this.loading = false);
    }
  },
  mounted() {
    this.fetchCategories();
    this.fetchProducts();
  }
})
</script>

<style scoped lang="scss">
@import "@/assets/css/colors.scss";
.title-container{
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 10%;

  .voegProductToeBtn{
    cursor: pointer;
    margin-left: 1rem;
    padding: 1rem;
    border-radius: 10px;
    background: $dark;
    display: flex;
    justify-content: center;
    align-items: center;
    color: $creme;
  }
}
.content-container{
  padding: 5rem 5rem 0 5rem;

  .items {
    display: flex;
    flex-wrap: wrap;
    gap: 30px;
  }
}

.editModalOuterContainer{
  width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
}
.modal{
  width: 100%;
  height: 100%;
}
.editModalContainer{
  padding: 0 2rem 0 2rem;
  margin: 0 auto;
  width: 500px;
  height: 100%;
  display: flex;
  flex-direction: column;

  label{
    margin-top: 1rem;
  }

  input, select{
    width: 90%;
    padding: 12px;
    border-radius: 4px;
  }
  select{
    width: 95%;
  }
  input[type=text]{
    border: 1px solid #ccc;
    width: 90%;
  }
}
.buttonContainer{
  margin-top: 15px;
  display: flex;
  gap: 10px;
  flex-direction: row;
  justify-content: space-around;
}
.productBtn{
  display: flex;
  justify-content: center;
  align-items: center;
  width: 40%;
  padding: 12px;
  border-radius: 4px;
  background: $dark;
  color: $creme;
  cursor: pointer;

  &:hover {
    background-image: linear-gradient(rgba(255, 255, 255, 0.1) 0 0);
  }
}
.deleteProductBtn{
  background: $red;
}
.fullWidth{
  width: 90% !important;
}
.disable{
  display: none !important;
}

.category{
  h2{
    cursor: pointer;
    img{
      width: 20px;
      height: 20px;
    }
  }
}

.collapsed{
  display: none !important;
}
.turnedArrow{
  transform: rotate(90deg);
}

</style>
