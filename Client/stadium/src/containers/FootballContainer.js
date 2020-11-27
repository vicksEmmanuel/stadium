import { Container } from "unstated";
import instance from '../helpers/axiosly';

class FootballContainer extends Container {
    
    constructor() {
        super();
        this.state = {
            teams: []
        }
    }

    getTeams = async () => {
        try {
            let teams = await instance.get("team/sport/1");
            if(teams.data?.isSuccess) {
                this.setState({...this.state, teams: teams.data.data})
            }
        } catch(e) { console.log(e)}
    }

    setTeam = (team) => {
        this.setState({...this.state, teams: this.state.teams.unshift(team)});
    }

}

export { FootballContainer }