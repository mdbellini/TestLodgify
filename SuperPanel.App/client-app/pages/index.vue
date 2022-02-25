<template>
  <div>
    <div class="h4 text-center m-4">
      Users List
    </div>
    <div class="row m-4">
      <div class="col-9">
        <b-form inline>
          <label class="sr-only" for="inline-form-input-name">Filter</label>
          <b-form-input
            id="inline-form-input-name"
            v-model="filter"
            class="mb-2 mr-sm-2 mb-sm-0"
            placeholder="Filter"
          />
        </b-form>
      </div>
      <div class="col-3">
        <b-pagination
          v-model="pageNumber"
          :total-rows="rowCount"
          :per-page="pageSize"
          aria-controls="tableUsers"
        />
      </div>
    </div>
    <div class="row mx-4">
      <div class="col">
        <b-table
          id="tableUsers"
          striped
          hover
          :busy="isLoading"
          :items="loadUsers"
          primary-key="id"
          :filter="filter"
          :fields="fields"
          :per-page="pageSize"
          :current-page="pageNumber"
        >
          <template #table-busy>
            <div class="text-center text-danger my-2">
              <b-spinner class="align-middle" />
              <strong>Loading...</strong>
            </div>
          </template>
          <template #cell(lastName)="data">
            <b-form-checkbox v-show="!data.value.isAnonymized" style="display:inline" :checked="userGDPRDeletionSelected(data.value.id)" @change="selectRow(data.value.id)" />
            {{ data.value.lastName }}
          </template>
          <template #cell(isAnonymized)="data" class="text-right">
            <b-button v-show="!data.value.isAnonymized" variant="primary" size="sm" @click="userGDPRDeletion(data.value.id)">
              GDPR deletion
            </b-button>
            <b-button v-show="data.value.isAnonymized" disabled variant="secondary" size="sm">
              Anonymized
            </b-button>
          </template>
        </b-table>
      </div>
    </div>
    <div class="row mx-4 my-1">
      <div class="col text-right">
        <b-button v-show="selectionCount>0" variant="primary" size="sm" @click="userGDPRDeletionMasive()">
          Process GDPR {{ selectionCount }} Users
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {

  data () {
    return {
      fields: [
        {
          key: 'lastName',
          sortable: true,
          sortDirection: 'desc',
          formatter: 'dataTemplate'
        },
        {
          key: 'firstName',
          sortable: true
        },
        {
          key: 'email',
          sortable: true
        },
        {
          key: 'login',
          sortable: true
        },
        {
          key: 'phone',
          sortable: false
        },
        {
          key: 'isAnonymized',
          sortable: false,
          formatter: 'dataTemplate'
        }

      ],
      isLoading: false,
      filter: null,
      pageNumber: 1,
      pageSize: 10,
      rowCount: 0,
      selection: []
    }
  },
  computed: {
    selectionCount () {
      return this.selection.length
    }
  },
  methods: {
    async loadUsers (ctx) {
      try {
        this.isLoading = true
        ctx.perPage = ctx.perPage || 10
        ctx.filter = ctx.filter || '||'
        ctx.sortBy = ctx.sortBy || '||'
        const url = `/api/Users/List?page=${ctx.currentPage}&size=${ctx.perPage}&filter=${ctx.filter}&sortBy=${ctx.sortBy}&sortDesc=${ctx.sortDesc}`

        const response = await axios.get(url)
        this.pageNumber = response.data.pageNumber
        this.rowCount = response.data.totalCount
        return response.data.items
      } catch (error) {
        console.log(error)
        return []
      } finally {
        this.isLoading = false
      }
    },
    dataTemplate (value, key, item) { // function to pass template values
      return { isAnonymized: item.isAnonymized, id: item.id, lastName: item.lastName }
    },
    async userGDPRDeletion (userId) {
      try {
        const url = `/api/Users/GDPRDeletion/${userId}`
        const response = await axios.get(url)
        if (response.status === 200) {
          if (response.data.result) {
            this.$root.$emit('bv::refresh::table', 'tableUsers')
          } else {
            alert(`Errors in GDPR Deletion: ${response.data.errors}`)
          }
        } else {
          alert(`Errors in GDPR Deletion: ${response.data.errors}`)
        }
      } catch (error) {
        console.log(error)
      }
    },
    selectRow (userId) {
      if (this.selection.includes(userId)) { this.selection.splice(this.selection.findIndex(uid => uid === userId), 1) } else { this.selection.push(userId) }
    },
    userGDPRDeletionSelected (userId) {
      return this.selection.includes(userId)
    },
    userGDPRDeletionMasive () {

    }
  }
}
</script>
