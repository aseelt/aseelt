import GetRandomSetService from 'axios'

export default{
    getSet(){
        return GetRandomSetService.get(`api/ViewById/Get_Random_Sets_Parts/`)
    }
}