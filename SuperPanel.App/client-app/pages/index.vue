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
    <div class="row m-4">
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
        </b-table>
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
          sortDirection: 'desc'
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
        }
      ],
      isLoading: false,
      filter: null,
      pageNumber: 1,
      pageSize: 10,
      rowCount: 0,
      users: [
      ]
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
    }
  }
}
</script>
