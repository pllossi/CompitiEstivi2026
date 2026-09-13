export class IRepository {

  async getAll() {
    throw new Error(`${this.constructor.name} deve implementare getAll()`);
  }

  async getById(id) {
    throw new Error(`${this.constructor.name} deve implementare getById()`);
  }

  async save(entity) {
    throw new Error(`${this.constructor.name} deve implementare save()`);
  }

  async delete(id) {
    throw new Error(`${this.constructor.name} deve implementare delete()`);
  }
}